---
name: Implement deep review feedback
description: Review deep review feedback provided by an independent reviewer, implement the necessary changes in the codebase, and respond to the reviewer with explanations of the changes made. This prompt is designed to ensure that all feedback from a deep review is thoroughly addressed and that the reviewer is kept informed of the changes implemented. The review feedback may include suggestions for code improvements, architectural changes, or other modifications to enhance the quality of the codebase, and may have come from either an actual human reviewer or an AI-based review tool. The goal is to ensure that all feedback is carefully considered and implemented where appropriate, and that the reviewer understands the rationale behind any changes made.
---

## How to use this prompt

The human will provide you with the feedback from a deep review, which may include multiple points of feedback across different areas of the codebase. You should carefully review each point of feedback, consider what the necessary changes to implement in the codebase based on the details. The details might even include specific details of what changes are needed including firm decisions on the way forward. However, before you do anything you should stop and check-in with the human.

## Understand the feedback

Make sure you have a clear understanding of the feedback provided. If any part of the feedback is unclear, ask the human for clarification before proceeding. It's important to fully understand the feedback and the necessary changes before you start implementing anything in the codebase. This will help ensure that you are making the right changes and that you are addressing all points of feedback effectively.

Also, make sure to consider the feedback in the context of the overall codebase and any relevant project goals or constraints. Some feedback might be based on specific assumptions or priorities, so it's important to understand the context in which the feedback was given.

Finally, consider that the feedback given is likely part of a much bigger review and that some feedback from elsewhere in the review has already been actioned. So, make sure to consider the feedback in the context of any other changes that have already been made or that are planned to be made based on other feedback from the review. This will help ensure that you are implementing changes that are consistent with the overall direction of the review and that you are not making conflicting changes based on different pieces of feedback.

## Confirm with the human what changes to make

After you've understood the feedback and the necessary changes, you should stop and ask the human what they actually want to happen. Even if you you've reviewed the feedback and it appears to includes very specific details of the changes needed, you should still stop and check-in with the human and ask them what they want to do.

In some cases, this might just be exactly the same as the feedback, but in other cases, the human might have specific preferences on how to implement the changes, want to provide additional context or guidance, or might want to prioritise certain changes over others. The human might also want to completely ignore the details in the feedback and do something different.

## Implement the changes

Once you have a clear understanding of the feedback and have confirmed with the human what changes to make, you should proceed to implement the necessary changes in the codebase. This may involve making code changes, refactoring existing code, or implementing new features based on the feedback provided. As you implement the changes, you should keep track of what changes you are making and why, so that you can provide a clear explanation to the reviewer later on.

IMPORTANT: Follow any and all repo guidelines and instructions for making changes to the codebase, including coding standards, commit message guidelines, and any other relevant processes. Especially important to use any custom agents as per the repo guidelines for making code changes, if such agents exist. For example, there might be a specific agent for coding tasks and a specific agent for updating documentation. Make sure to use the correct agent for each type of change you are making.
