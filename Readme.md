# T2 - Lifestyle Checker Web App

## Requirements Issue - Overlapping Age Ranges

See https://github.com/airelogic/tech-test-portal/issues/11

In https://github.com/airelogic/tech-test-portal/blob/main/T2-Lifestyle-Checker/Readme.md you have

AGE | 16 -21 | 22 - 40 | 41 -65 | 64+ -- | -- | -- | -- | --
This leaves the treatment of ages 64 and 65 ambiguous.

In this "solution" I will use
AGE | 16 -21 | 22 - 40 | 41 -64 | 65+

## Basic Approach

I've encountered more difficulties in this project than I was anticipating.
It is nearly twenty years since I was involved in high volume public-facing websites,
and I'm pleased to see that the domain is much richer now. 
In my current role I'm primarily dealing with scientific desktop-based applications, along with
calculation and reporting toolchains.
Our webapps are a cumbersome mix of an ancient 
Struts 2 java system, some Python Pyramid apps, more recent Oracle APEX CRUD systems,
and even some .php (I think there's even some perl being invoked from the .php). 
I didn't want to go down any of those roads for obvious reasons.

Given the role I'm seeking, use of C# seemed obvious, though I have never used ASP.NET previously.
Naively, never having used ASP.NET before, I looked at the problem and thought "that looks quite easy". 
The problem is, I think, but I wasn't aware of some of the steepness of ASP.NET's learning curve when approached without focussed training material and in the absence of a mentor.)
I therefore launched into work on part three early - the questionnaire XML and associated classes came quite quickly, as they were straightforward C# work.

Use of ASP.NET proved much more problematic for me - what you see here is the third project, following:
1) A doomed attempt at using Blazor. 
2) Two half-read long O'Reilly ASP.NET books, some poor material on learn.microsoft.com, and some useful but very basic material on w3schools...
3) A Razor page implementation which was beginning to come together until it suddenly stopped routing any pages, giving no errors (no stack traces,
no log files).  After hours of debugging and reading, I understood the issue to be a variant of one in which page routing to the all important Survey page was ambiguous.
I've only just managed to get the survey page rendering at all (hence its very poor appearance), and it currently has a HARD-WIRED assumed client age of 25.
This has allowed me to get the absolute basics of scoring together.

Further work is undoubtedly needed - at this stage however, I need to show you "something". The next move would be to adjust the redirect that gets to the page 
to pass a parameter (as an additional id???) and to revise the survey page's display logic.  After which we could naturally address appearance,
pre-submission validation, and dispose of the boiler plate navigation logic which is inappropriate in this context. The survey page should then be protected,
so that it can only be accessed via people who have passed the NHS number verification - at the moment there may be no explciit navigastion route there,
but the path is unprotected.

The code is certainly not ready for production.  I'll detail other issues I know of later.
The oddest thing - even though this has let me know I've clearly forgotten quite a few basics of web development, I've actually enjoyed the exercise.

## Basic Design Notes - Part One

Self post to Index.cshtml; the OnPostAsync() method processes the incoming form data, making a call to your web service using a static, pre-configured  HttpClient (see below - I'm aware of limitations here). 
Any JSON response is deserialized as an instance of the NHSPartialPatientDetails class - a test is then made using Match() method; Age computed and passed to
Questionnaire IsIneligable() method.


## Basic Design Notes - Parts Two and Three

The questionnaire is defined by LifestyleChecker.xml to provide a single place where all the requirements
of Part Two could be seen and double-checked.  This also gestures towards providing
support for other surveys, possibly with different scoring conditions for each question - though
clearly a full questionnaire system would need more flexibility than this.

At program start-up LifestyleChecker.xml is loaded into an instance of the Questionnaire class.
This mirrors the hierarchical structure of the XML :
- The Questionnaire is primarily a list of Question objects.
- Each Question has a list of possible Response objects.
- The scoring of each Response is determined by a list of ScoreClassification objects -
this has attributes determining its applicability and the applicable score if the response is selected.

The classes are implemented using XDocument and LINQ.

I've kept the processing of the scoring server-side, rather than doing it in JavaScript.

## Deployment

I regret that I haven't had time to seriously consider this, as I didn't have anything exhibiting any tangible progress until an hour ago.  
If using Visual Studio, build the Release version, Publish, adjust settings if necessary, and run the executable.  You'll also need to copy in an appropriate '_AccessToken.txt' file containing the token (alone, with no excess 
space or newline) to the release folder containing LifestyleSurvey.xml.  If I'd made greater progress, I'd've sought out a means of encrypting/de-crypting the token, since
it's clearly not great to leave raw access tokens lying around.  I considered and rejected the possibility of asking for the token from the command line, 
but that's ... ugly. I will update these instructions via email tomorrow morning.

Now, I need to find out how to use GitHub - it's just enough different from gitlab for me to have concerns.

## Status

I think the result should exhibit the basic functionality, but I am not happy with a number of
shortcuts I have been reduced to:
- I haven't had time to refine the user interface at all by .css styling and making use of javascript objects to give more interactivity 
(the pop-up datepicker is woefully inappropriate for going many years back in time, for example).
- In the event of validation failure on the NHS number form, any previously set variables are currently lost, which I'm sure could be rectified easily.
- My coding environment is not yet set up with tools like Selenium (or, better, Selenium Grid) to be able to
properly test the application.
- I am therefore unaware of any serious issues regarding scalability, and have only gone through
test scenarios on an ad hoc basis.
- I strongly suspect that my usage of HttpClient should have had greater sophistication (until yesterday I'd only used WebClient);
I couldn't get a suitable DI chain together to introduce MS's Http.Resilience extension mechanism.  I think this will leave
the app vulnerable to changes in Azure hosting due to DNS caching.
- It "would be nice" to have provided support for multiple end-user languages, though that was clearly beyond the brief.
- There's currently no collation of survey results to collect statistics.
- The project solution will contain excess artefacts based on the default project template.
- I was tempted to consider release via Docker, but that would be another learning curve.

In short, I'm not very content with what I'm sending you here.
I do hope that it provides some evidence of a degree of competency in C# and some ability to learn quickly on the job.

I'm submitting it despite all this, because I have enjoyed getting back into web development, and still believe I have other things to offer you.

I'd welcome feedback.