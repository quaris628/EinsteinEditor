# EinsteinEditor
An artful way to edit your bibite's brains!

__To download/install:__
 1. Go to https://github.com/quaris628/EinsteinEditor/releases/
 2. Select a version (1st number = release vs. pre-release, 2nd = enhancements, 3rd = bugfixes only)
 3. Download the EinsteinEditor_vX_X_X.zip file
 4. Exract the zip file to wherever you like (open your downloads folder, right-click the file > "Extract All..." > Select a location > Extract)

__To use:__
 1. Run /bin/Einstein.exe (or use the shortcut in the main folder)
    - Note: Windows will warn you about running this file because I'm not a verified publisher. You'll just have to trust me that I didn't write anything malicious in this. (And if it's any comfort, I do virus scans on all released files.)
    - If you want a shortcut on your desktop, right-click Einstein.exe in the file explorer > "Send To" > "Desktop (Create Shortcut)"

__To report bugs:__
 1. Create an issue here: https://github.com/quaris628/EinsteinEditor/issues/
 2. OR message me on reddit: u/quaris628
 3. OR email me: quaris314@gmail.com

 -----

What this can do:
 - Allow you to create and edit small, weak, inefficient, and dumb bibite brains
 - Crash spectacularly, or otherwise break (consider this a disclaimer)
 - Save/Load brain to/from a bibite file
 - Show/Hide input and output neurons in the diagram
 - Create/Delete hidden neurons
 - View types and descriptions of neurons in the diagram
 - Create/Delete synapses
 - Edit/View synapse strengths
 - Zoom and pan around the diagram
 - Drag neurons around the diagram
 - Handle window resizing (up to 2048x1080)
 - Auto-arrange neurons in a cleaner arrangement
 - Auto-fix non-unique neuron descriptions
 - Handle crashes with a prompt to report them and by logging info to a file

What this can't do (and probably never will):
 - Edit bibite genes
 - Edit world files
 - Make you a milkshake

What this sucks at doing (known bugs):
 - Counterproductive at helping you get a life
 - Synapse arrows don't always point their tips to the right place just after being created (moving either neuron it's attached to repositions it correctly) (probably won't be fixed)
 - Takes a long time to auto-fix duplicate neuron descriptions for large brains (probably won't be fixed) [technical note: could be improved by avoiding throwing and catching exceptions]

What this will do (planned enhancements):
 - Allow connecting a neuron to itself
 - Move keybinds info under a help button (and allow displaying it alongside the rest of the program)

What this might do (future ideas):
  Each feature has an estimated difficultly to create (1-5).
  You can vote for these features here: https://strawpoll.com/polls/B2ZBE3YYpgJ
 - Selecting neurons?
 - 1 - A clear-the-brain button
 - 1 - Have the default save/load folder location persist between closing and reopening the program
 - 2 - Keyboard shortcuts (in general)
 - 2 - Improve auto-arrangement (in general)
 - 2 - Look pretty (in general)
 - 2 - Allow multiple instances of just the contstant input neuron (same neuron in the bibite, two or more icons floating around in the diagram)
 - 3? - Allow multiple instances of all input neurons (same neuron in the bibite, two or more icons floating around in the diagram)
 - 3 - Toggle darkmode/lightmode
 - 3 - Stagger the spawn-in location of neurons (how?)
 - 3 - Edit neuron descriptions
 - 4 - Edit neuron type
 - 4 - Deactivate/activate synapses
 - 4 - Have positions of neurons persist between saving/loading brain to a bibite (Credit to Lucifer!)
 - 4 - Save/Load sub-assemblies of neurons and synapses. Could load multiple subassemblies into one brain.
 - 4 - Show detailed info about neurons (e.g. range of outputs for an input neuron, description of the effects of an output neuron, description of hidden neuron's behavior...)
 - 5 - Sentience (?)
 - 5 - Visualization of the flow of values throughout the network (or allow testing specific input values?)
 - 5 - Create color groups of neurons and synapses (Credit to Wazzah!)
    - Hide/Show certain color groups, or just make transparent
	- Drag and drop whole color groups
	- Delete a whole color group
 - 5 - Write notes/comments inside the diagram (like post-its)

 -----

To any potential contributors:
 - I want to talk with you about what you want to work on! (If it hasn't been too long since I wrote this, anyway.) In order of my preference, Discord: quaris#9905 Reddit: u/quaris628 Email: quaris314@gmail.com Carrier pigeon: Polly usually flies the fastest, but Gary is also pretty good.
 - I've been using Visual Studio 2019 for an IDE. Just FYI so that if you're running into problems with another IDE, you can always try switching as a workaround.
 - This project uses the phi graphics "library"/framework/whatever thing, which is a separate project I partnered in creating a few years ago. It has its own repository (https://github.com/quaris628/PhiGraphicsEngine), so if you spot any issues with or want to contribute to any of the code in the 'phi' folder, consider also going to that repo to write up an issue or pull request (or whatever). If you don't, I'll still try to keep the two in sync myself. [Edit: Given how many small fixes and enhancements I've been making, I'm just giving up on keeping the other repo in sync. I'll do a larger sync 'sometime later'.]
