# EinsteinEditor
An artful way to edit your bibite's brains!

__To download/install:__
 1. Go to https://github.com/quaris628/EinsteinEditor/releases/
 2. Select a version (1st number = release vs. pre-release, 2nd = enhancements, 3rd = bugfixes only)
 3. Download the EinsteinEditor_vX_X_X.zip file
 4. Exract the zip file to wherever you like (open your downloads folder, right-click the file > "Extract All..." > Select a location > Extract)

__To use:__
 1. Run /bin/Einstein.exe

If you want a shortcut on your desktop, right-click Einstein.exe in the file explorer > "Send To" > "Desktop (Create Shortcut)"

Note: Windows will warn you about running this file because I'm not a verified publisher. You'll just have to trust me that I didn't add anything malicious to it (intentionally or not). If it's any comfort, I do virus scans on all released files.

__To report bugs:__
(In order of my probable responsiveness)
 1. Visit the bibites research conglomerate discord server https://discord.gg/7TVF8X3X9H and ping @quaris
 2. OR Create a github issue: https://github.com/quaris628/EinsteinEditor/issues/
 3. OR email me: quaris314@gmail.com
 4. OR message me on reddit: u/quaris628

 -----

What this can do:
 - Allow you to create and edit small, weak, inefficient, and dumb bibite brains
 - Crash spectacularly, or otherwise break (consider this a disclaimer)
 - Save/Load brain to/from a bibite file
 - Show/Hide input and output neurons in the diagram
 - Create/Delete hidden neurons
 - Edit neuron descriptions, biases, and values
 - Create/Delete synapses (including connecting a neuron to itself)
 - Edit synapse strengths
 - Drag neurons around the diagram
 - Zoom and pan around the diagram
 - Have positions of neurons in the diagram persist between saving/loading (Credit to Lucifer!) - This happens by appending data to the end of neuron descriptions, which is the extra characters you'll see when viewing the brain in the bibites simulation.
 - Handle window resizing (up to 2048x1080)
 - Auto-arrange neurons in a cleaner arrangement
 - Auto-fix non-unique neuron descriptions
 - Support multiple bibite versions
 - Auto-convert brains between bibite versions (when saving to different-versioned files)
 - Simulate calculations of neuron values!
 - Support some mods for the bibites:
   - Strafing https://github.com/MeltingDiamond/Strafe-mod
   - Diet, Strength, and Defense neurons https://github.com/MeltingDiamond/Diet-Strength-and-Defence-nuron

What this can't do (and probably never will):
 - Edit bibite genes
 - Edit world files
 - Make you a milkshake
 - Show detailed info about input neurons (e.g. range of outputs for an input neuron, description of the effects of an output neuron...) - because it would be too difficult to keep all this info accurate for every different bibite version, especially as the simulation is updated and changed into the future

What this sucks at doing (known bugs):
 - Counterproductive at helping you get a life
 - Synapse arrows don't always point their tips to the right place just after being created (moving either neuron it's attached to repositions it correctly) (probably won't be fixed)
 - Synapse arrows that don't yet have a 'To' neuron don't move their base when its parent neuron is dragged (probably won't be fixed)
 - Takes a long time to auto-fix duplicate neuron descriptions for large brains (probably won't be fixed) [technical note: could be improved by avoiding throwing and catching exceptions]
 - If you convert a brain between bibite versions where a neuron with the same description as an input/output neuron that's in the new version but not the old one, then einstein crashes

What this might do (future ideas):
  Each feature has an estimated difficultly to create (1-5).
 - 1 - A clear-the-brain button
 - 1 - Have the default save/load folder location persist between closing and reopening the program
 - 2 - Keyboard shortcuts (in general)
 - 2 - Improve auto-arrangement (in general)
 - 2 - Look pretty (in general)
 - 3? - Allow multiple instances of all input neurons (same neuron in the bibite, two or more icons floating around in the diagram)
 - 3 - Toggle darkmode/lightmode
 - 3 - Show detailed info about hidden neuron behaviors (as in equations, graphs, or other descriptions)
 - 4 - Edit neuron type
 - 4 - Deactivate/activate synapses
 - 4 - Save/Load sub-assemblies of neurons and synapses. Could load multiple subassemblies into one brain.
 - 4 - Selecting neurons?
 - 4 - Hide/Show certain color groups, or just make transparent
 - 4 - Drag and drop whole color groups
 - 4 - Delete a whole color group
 - 4 - Write notes/comments inside the diagram (like post-its)
 - 5 - Sentience (?)
 - 5 - An undo button

 -----

To programmers who want to contribute:
 - I want to talk with you about what you want to work on and help you out! In order of my preference, Discord: quaris#9905 Email: quaris314@gmail.com Reddit: u/quaris628 and finally Carrier pigeon.
 - I've been using Visual Studio 2019 and 2022 for an IDE. Just FYI so that if you're running into problems with another IDE, you can always try switching as a workaround.
 - This project uses the phi graphics "library"/framework/whatever thing, which is a separate project I partnered in creating a few years ago. It has its own repository (https://github.com/quaris628/PhiGraphicsEngine), so if you spot any issues with or want to contribute to any of the code in the 'phi' folder, consider also going to that repo to write up an issue or pull request (or whatever). If you don't, I'll still try to keep the two in sync myself. [Edit: Given how many small fixes and enhancements I've been making, I'm just giving up on keeping the other repo in sync. I'll do a larger sync 'sometime later'.]

To modders who want to integrate:
 - Make your mod save .bb8s with a unique identifier in the "version" field, so that Einstein can identify this version. Format for the existing mods is: "vX.X.X modded: modName v1"
 - Make your mod allow loading .bb8s with these unique identifiers
   1. Find Utility.Version
   2. Find this line near the end of the file: 'private static Regex format = new Regex("^[0-9]+(\\.[0-9]+)(\\.[0-9]+)?([aA][0-9]+)?$");'
   3. Update that string to be "^[0-9]+(\\.[0-9]+)(\\.[0-9]+)?([aA][0-9]+)?( ?modded:.*)?$" (optionally, you can replace the ".*" with your mod's name if you want it to refuse to load other mods)
   4. It's possible you'll have to update the Parse function? Not updating it didn't seem to cause Melting Diamond any problems though.
 - Add support for your mod's different brains to Einstein.
   - If you can contact me (Quaris), I'll try to find time to do this for you!
   - if I can't do it for some reason... Take a look around einstein/config/bibiteVersions. Create a new bibite version class (see the bibiteVersions folder) that extends BibiteModdedVersion, and add it to ALL_VERSIONS in BibiteVersion. If you're doing something simple like adding an input/output neuron, you can look at how the existing versions (modded or unmodded) handle that and copy them. If you're doing more complex stuff, up to and including a complete overhaul of how brains work... good luck, I hope Einstein's code isn't unreadable!
