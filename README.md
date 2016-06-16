# PlethoraLaserCuttingMachine
Plethora Technical Exercise

Compile and run the application in Visual Studio 2013.

Go to Binaries/Release/ folder and run the ImportJsonApplication.exe

You can simply select the file you want and click the "Import" button or just drag and drop a file on the window.

Dependencies:

I used the Newtonsoft JSON nuget package and Prism6.

Comments:

I tried to test and think of multiple scenarios but I haven't tested the case of just havin circles, or just 1 segment with circles.
I know that I have this gap and I could spend more time to cover it.

If I had more time I'd definetely create unit tests for the parser and the geometric calculations and I'd create an installer for the application. I'd also maybe refactor the code a little bit as the Cost Calucation function is a bit big and I'd definetely add more and more meaningful error messages in case of an exception or error.

