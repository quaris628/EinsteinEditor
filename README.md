# EinsteinEditor
An artful way to edit your bibite's brains!

To download and use:
 [TODO: how to let people download and use it, and give them instructions, whenever I figure that out]

 -----

What this can do:
 - Allow you to create and edit small, weak, inefficient, and dumb bibite brains
 - Crash spectacularly, or otherwise break (consider this a disclaimer)
 - Handle crashes with a prompt to report them and by logging info to a file
 - Save/Load brain to/from a bibite file
 - Show/Hide input and output neurons in the editing area
 - Create/Delete hidden neurons (of specified types)
 - View types and descriptions of neurons
 - Create/Delete synapses
 - Edit/View synapse strengths
 - Drag neurons around the brain editor area
 - Handle window resizing (smaller than 1280 x 720)

What this can't do (and probably never will):
 - Edit bibite genes
 - Edit world files
 - Make you a milkshake

What this will do (planned features):
 - Bugfix: Don't crash when right-clicking the same neuron twice
 - Move neurons by click-click instead of dragging
 - When completing a synapse, don't start a new one
 - Adjust boundary box for edit area
 
What this might do (ideas for the future):
 - Look pretty
 - Have the default folder location persist between sessions
 - When neurons are added, have their spawn location staggered
 - A button to auto-arrange neurons in a prettier and more clean arrangement
 - Handle window resizing for larger sizes
 - Create a blank bibite
 - Keyboard shortcuts
 - Edit neuron descriptions
 - Edit neuron type
 - Get some sort of visual indicator of the flow of any arbitrary input combination? Or allow testing certain input values?
 - Zoom in/out
 - Sentience (?)
 
 -----

For any potential contributors:
 - I want to talk with you! (If it hasn't been too long since I wrote this, anyway.) You can message me on reddit at u/quaris628, or email me at quaris314@gmail.com.
 - I've been using Visual Studio 2019 for an IDE, so if you run into problems with another IDE, I'd try VS 2019 as a workaround.
 - This uses the phi graphics "library"/framework/whatever thing, which is a separate project I partnered in creating a few years ago. It has its own repository (https://github.com/quaris628/PhiGraphicsEngine), so if you spot any issues with or want to contribute to any of the code in the 'phi' folder, also go to that repo to write up an issue or pull request (or whatever). If you don't, I'll still try to keep the two in sync, it would just mean a little more work for me. [Edit: Given how many small fixes and enhancements I've been making, I'm just giving up on keeping the other repo in sync. I'll do a larger sync sometime in the future.]
