# EinsteinEditor
An artful way to edit your bibite's brains!

To download/install:
 1. Go to https://github.com/quaris628/EinsteinEditor/releases/
 2. Select a version
 3. Download the EinsteinEditor.zip file
 4. Exract the zip file to wherever you like (open your downloads folder, right-click the file > "Extract All..." > Select a location > Extract)

To use:
 1. Run EinsteinEditor/bin/Einstein.exe
   
Note: Windows will warn you about running this file because I'm not a verified publisher. You'll just have to trust me that I didn't write anything malicious in this.
(And if it's any comfort, I do virus scans on all released files.)

If you want a shortcut on your desktop, right-click Einstein.exe in the file explorer > "Send To" > "Desktop (Create Shortcut)"

Left-click to add or drag neurons. Middle-click neurons to remove.

Right-click neurons to add synapses. Middle-click synapses to remove.

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

What this sucks at doing (known bugs):
 - Counterproductive at helping you get a life
 - Whe loading from a file, synapses point to upper left corner
 - Completing a synapse starts a new one
 - Boundary for edit area isn't well aligned

What this will do (planned enhancements):
 - Remove neurons and synapses by shift+click instead of middle-click
 - When neurons are added, have their spawn location staggered (would probably also fix the synapses always pointing to the upper left when loading from a file)
 - A button to auto-arrange neurons in a prettier and more clean arrangement

What this might do (future ideas):
 - Sentience (?)
 - Have the default folder location persist between sessions
 - Somehow handle neurons with duplicate descriptions (assign new ones?)
 - Handle window resizing for larger sizes
 - Create a blank bibite
 - Allow bibite version to be configured (or auto-adjust?) (Not currently a strong need for this)
 - Keyboard shortcuts
 - Edit neuron descriptions
 - Edit neuron type
 - Create color groups for neurons !!!
    - Hide/Show certain color groups, or just make transparent
	- Drag and drop whole color groups
	- Delete a whole color group
 - Get some sort of visual indicator of the flow of any arbitrary input combination? Or allow testing certain input values?
 - Zoom in/out
 - Look pretty
 
 -----

To any potential contributors:
 - Thank you so much!
 - I want to talk with you! (If it hasn't been too long since I wrote this, anyway.) In order of my preference, Discord: quaris#9905 Reddit: u/quaris628 Email: quaris314@gmail.com
 - I've been using Visual Studio 2019 for an IDE. Just FYI so that if you're running into problems with another IDE, you can always try switching to VS 2019 as a workaround.
 - This project uses the phi graphics "library"/framework/whatever thing, which is a separate project I partnered in creating a few years ago. It has its own repository (https://github.com/quaris628/PhiGraphicsEngine), so if you spot any issues with or want to contribute to any of the code in the 'phi' folder, consider also going to that repo to write up an issue or pull request (or whatever). If you don't, I'll still try to keep the two in sync myself. [Edit: Given how many small fixes and enhancements I've been making, I'm just giving up on keeping the other repo in sync. I'll do a larger sync sometime around when I do a full release of the Einstein Editor.]
