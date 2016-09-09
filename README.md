# TryIt
### A .Net library for retrying services and gracefully falling back when things go wrong.
##### By Joe Cleland
In today’s world of cloud based development, it’s more and more important for developers to write robust code that can gracefully handle temporary service outages, failures, and the like. But writing (and testing) this code is tedious, time-consuming and just plain boring. 

That’s where **TryIt** comes in. TryIt is a .Net library that allows you to `Try` your services a number of times before giving up. TryIt uses a simple fluent style so your code is readable and easy to understand. Yet with its optional **`UsingDelay`**, **`ThenTry`**, **`WithErrorPolicy`**, and **`WithSuccessPolicy`** methods it’s powerful enough to handle very sophisticated retry scenarios.

### Where to I Get It?
#### You must have [.Net 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653) or higher
+ Download the [**NuGet package**](http://www.nuget.org)
+ Download the [**source code from GitHub**](https://github.com/yozepi/TryIt)

### How can I contribute?

Please refer to the [these guidelines](CONTRIBUTING.md) if you wish to ask questions, submit bugs, or contribute to new features.

# So Let’s TryIt!
TryIt will accept any .Net **Action** or **Func**. Which means it accepts almost any method you write as input* (methods that return **Tasks** are a special sort of beast so we’ll explain in detail about how to use those [**later**](#tasks)).

<sub>* the current version of TryIt allows Actions and Funcs with up to 4 parameters. More parameters will be allowed in future versions.</sub>

Let’s say you have an existing method that downloads the contents of a website as a string:
```c#
string result = DownloadString(url);
```
But this method calls a service that often fails intermittently.  Here’s how TryIt can instantly make you’re your code more robust: 

First, include this using statement at the top of your code:
```c#
using Retry;
using Retry.Delays;
```
Then replace
```c#
string result = DownloadString(url);
```
With this:
```c
string result = TryIt.Try(DownloadString, url, 5).Go();
```
This code will cause TryIt to try your method 5 times or until it succeeds. Whichever comes first. If every try fails, **TryIt** will raise a **`RetryFailedException`** with the exceptions from each failed try in it's **`ExceptionList`** property.

Ok that was simple. But typically if a method fails once then chances are it’s going to fail again a microsecond or two later. That’s why TryIt allows you to add a pause between tries using the **`UsingDelay`** method. Look at this:
```c#
string result = TryIt.Try(DownloadString, url, 5)
    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
    .Go();
```
Now your method will pause between each try. Notice how the `UsingDelay` method is chained into the call. This is typical syntax for **TryIt**. You can chain all of it's methods together in this fashion.

In this case a back-off delay is used. Using this delay, TryIt will double the wait time for each try, 200 milliseconds the first time, 400 the next, then 800, and so on. The current version of **TryIt** comes out of the box with **`BackoffDelay`** and the **`BasicDelay`** - which simply pauses for a set timespan between tries. If what comes out of the box doesn't fit your needs you can inherit from the abstract **`Delay`** class to create your own.

# TryIt Again
So what if you want to try with one delay 3 times and then another delay for 5 times? Or what if you need to try server A, and if that fails try server B? That’s where the `ThenTry` method comes in. Look at the code below:
```c#
string result = TryIt.Try(DownloadString, url, 3)
    .ThenTry(5)
    .UsingDelay(Delay.Basic(TimeSpan.FromMilliseconds(200)))
    .Go();
```
In this example DownloadString is called 3 times back to back with no delay and then called another 5 times using a 200 millisecond
delay between each call.

Check this out:
```c#
string result = TryIt.Try(DownloadString, urlA, 4)
   .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
   .ThenTry(urlB, 4)
   .Go();
```
This code calls DownloadString using urlA four times and with a delay between each try. If none of those tries succeed it then calls 
DownloadString using urlB four times using the same delay. Note how TryIt lets you pass alternate parameters to your method! 
If you have a urlC, you can chain another ThenTry to the list. 

### A Gotcha
```c#
string response = TryIt.Try((DownloadString, urlA, 4)
    .ThenTry(urlB, 4)
    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
    .Go();
```
This code looks similar to the code above but there’s a **_subtle difference_**. Notice how `UsingDelay` is placed after `ThenTry`. This causes TryIt to first try using urlA *with no delay* then retry using urlB with the delay.

Here’s why this happens: `Try` and `ThenTry` methods chain a series of “runners” together (they’re called runners because they “run” your action or method). As TryIt chains each runner together, it uses the values set from the previous runner in the chain. When `UsingDelay` is called (or any other method for that matter), the property of the last runner in the chain gets updated. And every runner after that uses the new value. So in the example above, urlA is tried using the default delay (which is no delay by default) then urlB is tried using the delay provided in the UsingDelay method.

# Go (and GoAsync)

I’m sure you’ve noticed the `Go` method at the end of each of these examples. TryIt works by building up a chain of “runners” that actually do the job of trying your Action/Method. TryIt needs a way to know that all the runners have been chained together and that its time to begin. That’s what `Go` is for. `Go` will start the chain and will return the results of your method (if it returns results). `GoAsync` will return an awaitable **`Task`** (or **`Task<TResult>`**) that runs the chain.

# Alternate Actions/Funcs
In addition to providing alternate parameters to your methods, you can provide *alternate methods* to try! The only caveat is that you need to be sure your alternate method returns the same type as the original method. Using an alternate method is how you can provide fallback results in case of failures.

In this example, if the Download method fails 3 times, a method that returns alternate content gets called.
```c#
var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
string result = TryIt.Try(DownloadString, url, 3)
   .UsingDelay(backoff)
   .ThenTry(GetAlternateContent, 1)
   .Go();
```

# Breaking the Chain
### Breaking out of the Try-ThenTry sequence
There are going to be times when you don’t want to keep retrying a method. For example, you may not want to call a service multiple times if the service is unavailable. This is where the **`WithErrorPolicy`** method comes into play. **`WithErrorPolicy`** is a method that accepts an **`ErrorPolicyDelegate`** you can use to perform special handling when a try fails. You can use it to either stop trying and move on to the next runner (`ThenTry`) or to break out of the chain completely. It’s also not a bad place to log exceptions from your actions as they occur. Refer to the table below to see how TryIt behaves depending on what you do in the **`ErrorPolicyDelegate`**:

|When ErrorPolicyDelegate does this|TryIt does this|
|----------------------------------|--------------|
|Return true                       |TryIt processes the exception as normal (as if there we’re no error policy).|
|Return false                      |TryIt stops trying the current runner, wraps the exception into a ErrorPolicyException and adds it to the exception list, then moves to the nextrunner. If there are no more runners then a RetryFailedException is thrown.|
|Throw an exception                |TryIt raises the exception. All remaining runners are ignored and will not be tried.|

Let’s see an example. We’ll use the DownloadString method from our previous examples. Let’s say we don’t control the url passed to us. If the url can’t be resolved, then we certainly don’t want to waste our time retrying it.  Here’s how you can set up TryIt to handle this:
```c#
string result = TryIt.Try(DownloadString, url, 4)
    .WithErrorPolicy((ex, retries) =>
    {
        var policyEx = ex as WebException;
        if (policyEx?.Status == WebExceptionStatus.NameResolutionFailure)
            throw (ex);
        return true;
    })
   .UsingDelay(backoff)
   .Go();
```
TryIt will call the **`ErrorPolicyDelegate`** when an exception occurs in the DownloadString method. The delegate accepts two parameters: the exception that caused the failure and the count of tries so far (for this runner). Looking at the delegate instance above you can see that, if the exception happens to be a `WebException` whose status is `NameResolutionFailure`, then the delegate will throw the exception. Otherwise it returns true. If the exception is thrown, TryIt will stop all processing and throw the exception.

Look at this slightly different example:
```c#
string response = TryIt.Try(DownloadString, url, 4)
    .WithErrorPolicy((ex, tryCount) =>
    {
        var policyEx = ex as WebException;
        if (policyEx?.Status == WebExceptionStatus.NameResolutionFailure)
            return false;
        return true;
    })
    .UsingDelay(backoff)
    .Go();
```
This code is almost identical. Except, instead of throwing the exception, the delegate just returns false. This causes TryIt to stop executing the current runner, wrap the exception into a `ErrorPolicyException`, and (in this case) throw a `RetryFailedException`. `RetryFailedException` gets thrown because, in this case, there’s only one runner. (`Try`, with no `ThenTry`).

If you were to add a ThenTry,  the error policy would carry over to it as well. Look at this:
```c#
var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
string result = TryIt.Try(DownloadString, urlA, 4)
  .UsingDelay(backoff)
  .WithErrorPolicy((ex, tryCount) =>
  {
      var policyEx = ex as WebException;
      if (policyEx?.Status == WebExceptionStatus.NameResolutionFailure)
          return false;
      return true;
  })
  .ThenTry(urlB, 4)
  .Go();
```
This is similar to our previous example where we show how to pass alternate parameters. The only difference is that we’ve added our error policy delegate. This code will try DownloadString using urlA. If using urlA fails after 4 tries *or* DownloadString throws a WebException because of a NameResolutionFailure, TryIt will continue to try DownloadString with urlB – using the same error policy.

If you like you can switch policies just by calling **`WithErrorPolicy`** with a new **`ErrorPolicyDelegate`**
```c#
string result = TryIt.Try(DownloadString, urlA, 4)
  .UsingDelay(backoff)
  .WithErrorPolicy((ex, tryCount) => errorPolicyA)
  .ThenTry(urlB, 4)
  .WithErrorPolicy((ex, tryCount) => errorPolicyB)
  .Go();
```
This code will use errorPolicyA for urlA and errorPolicyB for urlB. 

If you want to stop all error policies just pass `null` to the **`WithErrorPolicy`** method.
```c#
string result = TryIt.Try(DownloadString, urlA, 3)
  .UsingDelay(backoff)
  .WithErrorPolicy((ex, tryCount) => errorPolicyA)
  .ThenTry(urlB, 3)
  .WithErrorPolicy(null)
  .Go();
```
This code will still use errorPolicyA for urlA but will use no error policy for urlB.

# Sometimes a Success Is a Fail
### Customizing success scenarios

By default, TryIt considers any Action or Func processing that does not throw an exception as being a success. But this isn’t always the case. Sometimes you have to examine the results to determine a success or a failure. This is what the **`WithSuccessPolicy`** method is used for.

The **`WithSuccessPolicy`** method accepts a **`SuccessPolicyDelegate`** instance that you provide. When TryIt succeeds, it calls your **`SuccessPolicyDelegate`**, passing in the results of your method (if your method returns results). Throwing an exception inside your **`SuccessPolicyDelegate`** instance will cause TryIt treat your exception as a failure and continue to retry. If you want TryIt to stop retrying, you’ll have to capture your exception using **`WithErrorPolicy`** and deal with it there.

Here’s an example of this:
```c#
string result = TryIt.Try(DownloadString, url, 3)
    .UsingDelay(backoff)
    .WithSuccessPolicy((result, trycount) =>
    {
        if (string.IsNullOrEmpty(result))
            throw new InvalidOperationException("Unacceptable results!");
    })
    .WithErrorPolicy((ex, tryCount) =>
    {
        if (ex.GetType() == typeof(InvalidOperationException))
            return false;
        return true;
    })
    .Go();
```
In this example the `SuccessPolicyDelegate` throws and InvalidOperationException if the result of the DownloadString method is null or empty. Then `ErrorPolicyDelegate` captures that exception and cancels further retries. 

<a name="tasks"></a>
# Retrying Tasks
In short, you can't. Once a Task has completed it can't be restarted. **But you can retry _functions_ that return Tasks!**

TryIt will deal with these functions differently than it does a function that returns any other value (say, an `int` for example). In most cases TryIt will return the result of the function. But, _for functions that return Tasks_, **TryIt will run the returned Task and return the result of the Task!** This makes sense if you think about it.


When a Task is returned from a function or method call, it's completion state is unknown. Even if a Task fails (or is going to fail) the method still returns a Task. So there's no way for TryIt to know how to handle the task. it must make it's decisions based on the Task's results.

For this reason, TryIt (asynchronously) runs the Task and examines it's results.

In the following example TryIt tries the `DownloadStringAsync` method - which returns a Task&lt;string&gt;:
```c#
string result = TryIt.TryTask(DownloadStringAsync, url, 5)
    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
    .Go();
```
TryIt will run `DownloadStringAsync` in the background while your code waits. Once a result is received, that result is returned to your code.

### But You need to return a `Task<string>` you say?
No problem. Use `GoAsync` instead of `Go`.
```c#
Task<string> response = TryIt.TryTask(DownloadStringAsync, url, 3)
    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
    .GoAsync();
```
This code returns a `Task<string>` that runs your TryIt chain and returns results.

## License

**TryIt** is licensed under [Apache License, Version 2.0](https://opensource.org/licenses/Apache-2.0).

## Acknowedgements

Thanks to [Vishwas Lele](http://app.pluralsight.com/author/vishwas-lele). Your Pluralsight course titled ["Cloud Oriented Programming"](https://app.pluralsight.com/library/courses/cloud-oriented-programming/table-of-contents) made me recognize the need for tools like TryIt.

Thanks to [NewtonSoft.Json](https://github.com/JamesNK/Newtonsoft.Json). Your awesome CONTRIBUTING.md file helped me a lot with setting contribution guidelines for this project.

Thanks to [NSpec](http://nspec.org/) for making a great tool for creating spec style tests.

Thanks to [Pieter Gheysens](https://intovsts.net/about/) for his [blog post](https://intovsts.net/2015/08/24/tfs-build-2015-and-versioning/) that inspired me to create automatic assembly versioning during automated builds. 

Thanks to [Ben Day](https://www.benday.com/author/admin/) for his [script](https://www.benday.com/2016/01/15/learn-to-customize-tfs2015-build-with-environment-variables/) for exporting Team Services environment variables.
