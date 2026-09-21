# Build or preview the Jekyll documentation site via Docker/Podman (no local Ruby/Jekyll install required).
# Usage: .\scripts\Invoke-DocsSite.ps1 build|serve|preview|stop

param(
    [Parameter(Position = 0)]
    [ValidateSet("build", "serve", "preview", "stop")]
    [string]$Command = "build",

    [string]$Runtime = "podman",
    [int]$ServePort = 4000,
    [int]$PreviewPort = 8080
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rootForMount = $repoRoot -replace '\\', '/'
$docsDir = Join-Path $repoRoot "docs"
$sitePath = Join-Path $docsDir "_site"
$jekyllImage = "docker.io/jekyll/jekyll:4"
$nginxImage = "docker.io/library/nginx:alpine"
$serveContainerName = "freeagent-docs-serve"
$previewContainerName = "freeagent-docs-preview"
$bundleSetupCmd = "bundle config set --local path vendor/bundle && bundle install --jobs 4 --retry 3"
$localHost = "127.0.0.1"

function Test-ContainerRuntime {
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Write-Error "Container runtime '$Name' not found. Install Podman Desktop or Docker Desktop, or pass -Runtime docker."
    }
}

function Stop-Container {
    param([string]$Name)
    & $Runtime container exists $Name 2>$null
    if ($LASTEXITCODE -eq 0) {
        & $Runtime stop -t 3 $Name 2>$null
        & $Runtime rm -f $Name 2>$null
    }
}

function Stop-ContainersOnPort {
    param([int]$Port)
    $ids = & $Runtime ps --filter "publish=$Port" --format '{{.ID}}' 2>$null
    foreach ($id in $ids) {
        if ([string]::IsNullOrWhiteSpace($id)) { continue }
        Write-Host "Stopping container $id (port $Port)..." -ForegroundColor DarkGray
        & $Runtime stop -t 3 $id 2>$null
        & $Runtime rm -f $id 2>$null
    }
}

function Assert-PortAvailable {
    param([int]$Port)
    Stop-ContainersOnPort -Port $Port
    $listener = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    if ($listener) {
        Write-Error "Port $Port is still in use. Run: .\scripts\Invoke-DocsSite.ps1 stop"
    }
}

function Stop-AllDocsContainers {
    Stop-Container -Name $serveContainerName
    Stop-Container -Name $previewContainerName
    Stop-ContainersOnPort -Port $ServePort
    Stop-ContainersOnPort -Port 35729
    Stop-ContainersOnPort -Port $PreviewPort
}

function Invoke-Jekyll {
    param(
        [string[]]$JekyllArgs,
        [string[]]$ExtraRunArgs = @(),
        [string]$ContainerCommand
    )

    $runArgs = @("run", "--rm") + $ExtraRunArgs + @(
        "-v", "${rootForMount}/docs:/srv/jekyll:Z",
        "-w", "/srv/jekyll",
        $jekyllImage,
        "sh", "-c", $ContainerCommand
    )
    & $Runtime @runArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Jekyll failed (exit $LASTEXITCODE). Fix errors above before continuing."
    }
}

function Assert-SiteIndex {
    if (-not (Test-Path (Join-Path $sitePath "index.html"))) {
        Write-Error "Jekyll did not produce docs/_site/index.html."
    }
}

function Wait-ForHttp {
    param([string]$Url)
    for ($attempt = 1; $attempt -le 60; $attempt++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 2
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 400) {
                return
            }
        }
        catch {
            Start-Sleep -Seconds 1
        }
    }
    Write-Error "Timed out waiting for $Url"
}

function Follow-ContainerLogs {
    param(
        [string]$Name,
        [int]$Port,
        [int]$ExtraPort = 0
    )

    try {
        & $Runtime logs -f $Name
    }
    finally {
        Write-Host ""
        Write-Host "Stopping..." -ForegroundColor DarkGray
        Stop-Container -Name $Name
        Stop-ContainersOnPort -Port $Port
        if ($ExtraPort -gt 0) {
            Stop-ContainersOnPort -Port $ExtraPort
        }
    }
}

Test-ContainerRuntime -Name $Runtime

switch ($Command) {
    "build" {
        Write-Host "Building Jekyll site to docs/_site..." -ForegroundColor Cyan
        $buildCmd = "$bundleSetupCmd && bundle exec jekyll build --destination _site"
        Invoke-Jekyll -ContainerCommand $buildCmd
        Assert-SiteIndex
        Write-Host "Done. Output: $sitePath" -ForegroundColor Green
    }

    "serve" {
        Stop-Container -Name $serveContainerName
        Assert-PortAvailable -Port $ServePort
        Assert-PortAvailable -Port 35729
        Write-Host "Starting Jekyll dev server at http://${localHost}:$ServePort/ ..." -ForegroundColor Cyan
        Write-Host "On WSL, use 127.0.0.1 rather than localhost." -ForegroundColor DarkGray
        Write-Host "Press Ctrl+C to stop (or run: .\scripts\Invoke-DocsSite.ps1 stop)" -ForegroundColor DarkGray
        $serveCmd = "$bundleSetupCmd && exec bundle exec jekyll serve --host 0.0.0.0 --port 4000 --baseurl '' --livereload --livereload-port 35729 --force_polling"
        & $Runtime run -d --rm --init --name $serveContainerName `
            -p "${localHost}:${ServePort}:4000" `
            -p "${localHost}:35729:35729" `
            -v "${rootForMount}/docs:/srv/jekyll:Z" `
            -w /srv/jekyll `
            $jekyllImage `
            sh -c $serveCmd
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to start Jekyll container (exit $LASTEXITCODE)."
        }
        Wait-ForHttp -Url "http://${localHost}:$ServePort/"
        Write-Host "Server ready." -ForegroundColor Green
        Follow-ContainerLogs -Name $serveContainerName -Port $ServePort -ExtraPort 35729
    }

    "preview" {
        Stop-Container -Name $previewContainerName
        Assert-PortAvailable -Port $PreviewPort
        Write-Host "Building Jekyll site for local preview..." -ForegroundColor Cyan
        $previewBuildCmd = "$bundleSetupCmd && bundle exec jekyll build --destination _site --baseurl ''"
        Invoke-Jekyll -ContainerCommand $previewBuildCmd
        Assert-SiteIndex
        Write-Host "Serving docs/_site at http://${localHost}:$PreviewPort/ ..." -ForegroundColor Cyan
        Write-Host "On WSL, use 127.0.0.1 rather than localhost." -ForegroundColor DarkGray
        Write-Host "Press Ctrl+C to stop (or run: .\scripts\Invoke-DocsSite.ps1 stop)" -ForegroundColor DarkGray
        & $Runtime run -d --rm --init --name $previewContainerName `
            -p "${localHost}:${PreviewPort}:80" `
            -v "${rootForMount}/docs/_site:/usr/share/nginx/html:ro,Z" `
            $nginxImage
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to start preview container (exit $LASTEXITCODE)."
        }
        Wait-ForHttp -Url "http://${localHost}:$PreviewPort/"
        Write-Host "Preview ready." -ForegroundColor Green
        Follow-ContainerLogs -Name $previewContainerName -Port $PreviewPort
    }

    "stop" {
        Stop-AllDocsContainers
        Write-Host "Stopped documentation containers." -ForegroundColor Green
    }
}
