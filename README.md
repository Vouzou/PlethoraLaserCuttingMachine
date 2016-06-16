# PlethoraLaserCuttingMachine
Plethora Technical Exercise

Compile and run the application in Visual Studio 2013. Set as Start Up project the ImportJsonApplication project.

Go to Binaries/Release/ folder and run the ImportJsonApplication.exe

You can simply select the file you want and click the "Import" button or just drag and drop a file on the window. You can drag and drop one file after the other. If you drag multiple files it will just process the first one.
I added a folder "Profiles" with the modified json files. I modified the files so they comply with the schema:
{
    "Edges": [
        id: {
            "Type": "LineSegment",
            "Vertices": [id],
        },
        id: {
            "Type": "CircularArc",
            "Center": {
                "X": double,
                "Y": double,
            },
            "ClockwiseFrom": id,
            "Vertices": [id],
        }
    ],
    "Vertices": [
        id: {
            "Position": {
                "X": double,
                "Y": double,
            }
        }
    ]
}

Dependencies:

I used the Newtonsoft JSON nuget package and Prism6.

Comments:

I tried to test and think of multiple scenarios but I haven't tested the case of just havin circles, or just 1 segment with circles.
I know that I have this gap and I could spend more time to cover it.

If I had more time I'd definetely create unit tests for the parser and the geometric calculations and I'd create an installer for the application. I'd also maybe refactor the code a little bit as the Cost Calucation function is a bit big and I'd definetely add more and more meaningful error messages in case of an exception or error.

