# Contributing
### Please help make TryIt better!
Issue reporting, bug fixes, document patches and feature updates are always welcome! Please read these guidelines before contributing to TryIt:

+ [Questions or Problems](#questions)
+ [Issues and Bugs](#bugs)
+ [Feature Requests](#features)
+ [Submitting a Pull Request](#pull)
+ [Contributor License Agreement](#license)

<a name="questions"></a>
## Questions or Problems
If you have any questions about TryIt that can't be answered in the [readme file](https://github.com/yozepi/TryIt/blob/master/README.md) or in the wiki (coming soon), please ask your question on [Stack Overflow](http://stackoverflow.com/questions/tagged/tryit). Be sure to tag your question with the **`TryIt`** tag.

The GitHub Issues section is only for [reporting bugs](#bugs) and [feature requests](#features). Please don't ask help related questions there.

<a name="bugs"></a>
## Issues and Bugs
f you find a bug in the source code or a mistake in the documentation, you can help by submitting an issue to the [GitHub Repository](https://github.com/yozepi/TryIt/issues). Even better you can submit a [Pull Request](#pull) with a fix.

When submitting an issue please include the following information:
+ A description of the issue
+ The exception message and stacktrace if an error was thrown
+ If possible, please include code that reproduces the issue. [DropBox](https://www.dropbox.com/) or GitHub's [Gist](https://gist.github.com/) can be used to share large code samples, or you could submit a [pull request](#pull) with the issue reproduced in a new test.

The more information you include the more likely it will be to get fixed!

<a name="features"></a>
## Feature Requests
You can submit a new feature request for TryIt on the [GitHub Repository](https://github.com/yozepi/TryIt/issues). When submitting a feature request, consider the following:
+ Can TryIt already do it? TryIt's `WithErrorPolicy` and `WithSuccessPolicy` methods offer a lot of flexability.
+ Will it break what already works? Consistency and stability between versions is very important. Any requests that will cause breaking changes should be accompnied with a compelling reason as to why the breaking change should be made.

<a name="pull"></a>
## Submitting a Pull Request
When submitting a pull request to the [GitHub Repository](https://github.com/yozepi/TryIt/pulls) make sure to do the following:
+ Check that new and updated code follows TryIt's existing code formatting and naming standard
+ Run TryIt's unit tests to ensure no existing functionality has been affected
+ Write new unit tests to test your changes. All features and fixed bugs must have tests to verify they work
+ When writing unit tests, use the Nspec testing framework (see existing tests for examples)
+ Please keep test coverage at 100% for shippable code.

Read [GitHub Help](https://help.github.com/articles/about-pull-requests/) for more details about creating pull requests.

<a name="license"></a>
## Contributor License Agreement
By contributing your code to TryIt you grant Joe Cleland a non-exclusive, irrevocable, worldwide, royalty-free, sublicenseable, transferable license under all of Your relevant intellectual property rights (including copyright, patent, and any other rights), to use, copy, prepare derivative works of, distribute and publicly perform and display the Contributions on any licensing terms, including without limitation: (a) open source licenses like the MIT license; and (b) binary, proprietary, or commercial licenses. Except for the licenses granted herein, You reserve all right, title, and interest in and to the Contribution.

You confirm that you are able to grant us these rights. You represent that You are legally entitled to grant the above license. If Your employer has rights to intellectual property that You create, You represent that You have received permission to make the Contributions on behalf of that employer, or that Your employer has waived such rights for the Contributions.

You represent that the Contributions are Your original works of authorship, and to Your knowledge, no other person claims, or has the right to claim, any right in any invention or patent related to the Contributions. You also represent that You are not legally obligated, whether by entering into an agreement or otherwise, in any way that conflicts with the terms of this license.

Joe Cleland acknowledges that, except as explicitly described in this Agreement, any Contribution which you provide is on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING, WITHOUT LIMITATION, ANY WARRANTIES OR CONDITIONS OF TITLE, NON-INFRINGEMENT, MERCHANTABILITY, OR FITNESS FOR A PARTICULAR PURPOSE.
