# VR Portal Example for Meta Quest

*Copyright 2020-2023 Bart Trzynadlowski*

A very simple example of how to create a portal effect in VR using Unity and Meta Quest. This method creates two virtual cameras,
one for each eye, rendering to two render targets. There are almost certainly more performant ways of doing this but this method
is a good starting point and easy to understand.

Use the left joystick to move around. You can also use the keyboard (WASD, arrow keys, page up/down) to move around when playing
in the editor. You can walk through the portal although this could use some work to appear more seamless.

![Board connections](media/Demo.gif)

**Update: April 18, 2023**

A latency issue causing the portal to appear "wobbly" due to using the previous frame's head pose has, for the time being,
been fixed by using `LateUpdate` instead of `Update` in `PortalCamera.cs`. I *thought* I had tried this before but it does seem to
work now.