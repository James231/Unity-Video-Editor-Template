# Unity Video Editor Template

A template for Unity 2019.3 or later, which lets you use Unity as a Video Editor.

To get an understanding of why you might do this, and what can be achieved watch:  
[Example YouTube Video](https://youtu.be/47NaGi8E7UI)

For a more complete explaination/documentation read this blog post:  
[Read Blog Post](http://blog.jam-es.com/2020/06/making-youtube-videos-with-unity.html)

## Getting Started

In 5 simple steps:  
1. Clone/download the repository and open it in Unity 2019.3 or later.  
2. Open the Sample Scene and set the output video resolution by changing the Game View resolution (use 1920x1080 for the xample scene).  
3. Open the Timeline window and select the 'Global Timeline' object.  
4. Enter Playmode to watch the video play. The output is recorded during the 'RecorderClip' within timeline.  
5. Open the 'Recordings' folder which will be created in the project directory and will contain the exported video.  
  
Please star :star: the repository if you find it useful so others can find it.

## Tips

1. The project includes a 'Welcome' window to help get you setup. It can also be accessed through the Top Menu Bar.  
2. Use Timeline! It makes everything much easier. Plus the included modifications to fix VideoPlayer bugs are only designed for use with Timeline.  
3. All video clips you use should have 'Transcode' checked in their import settings. This will prevent delays on videos starting to play.  
4. Don't worry about dropping frames. Unity Recorder won't let that happen. So you can export your videos on lower-end devices if you want. The export process will take longer but you won't lose quality.  
5. Use TextMeshPro for all text you use to make sure it is crystal clear. The project includes some popular fonts which are ready to use from [Google Fonts](https://fonts.google.com/).
6. Don't start recording as soon as you enter playmode! In Timeline leave a few seconds before the RecorderClip starts (just like in the sample scene). This is important because some video thrumbnail processing is done in the first second when you enter Playmode.


## References

 
1. The project uses the [Unity Recoreder Package](https://docs.unity3d.com/Packages/com.unity.recorder@2.0/manual/index.html) to record the contents of the 'Game View' during Playmode to create the video. You don't need to build anything! The recording can be done in multiple ways but I recommend using a [RecorderClip through Timeline](https://docs.unity3d.com/Packages/com.unity.recorder@2.0/manual/RecordingTimelineTrack.html) (see the sample scene). The package is still in alpha so you might want to update it later on.  
2. The project also uses the [Default Playables](https://assetstore.unity.com/packages/essentials/default-playables-95266) package from the Asset Store. But, this package has been heavily modified to make it compatible with Unity Recorder, so **do not update it!**  
3. The sample video includes UI elements with colour gradients. The code came from [this GitHub repo](https://github.com/azixMcAze/Unity-UIGradient). The sample scene also uses some avatar characters from [here](https://hdwallsbox.com/avatar-maker/) and the soundtrack comes from YouTube's free music library. It is a a track by Futuremono called 'Long Road'.

## License

This code is released under MIT license. Modify, distribute, sell, fork, and use this as much as you like. Both for personal and commercial use. I hold no responsibility if anything goes wrong.

If you use this, you don't need to refer to this repo, or give me any kind of credit but it would be appreciated. At least a :star: would be nice.

## Contributing

Pull Requests are welcome. But, note that by creating a pull request you are giving me permission to merge your code and release it under the MIT license mentioned above. At no point will you be able to withdraw merged code from the repository, or change the license under which it has been made available.

## Need Support?

You can contact me by submitting a contact forms [here](https://jam-es.com) or [here](https://solutionstudios.jam-es.com/contactus) or by opening an issue. If using the contact forms please state you are asking about the 'Video Editor Template' to avoid confusion.

I also do freelance work if you need something more significant.

## Support Me

If you are a Unity user please consider purchasing some of my other Unity assets which you can find [here](https://solutionstudios.jam-es.com/unityassets). Or contact me through the forms linked above to donate some other way.