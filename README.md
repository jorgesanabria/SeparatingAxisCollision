# SeparatingAxisCollision.MonoGame
## simple SAT-based collision detection and response library for MonoGame 3.6

You will need Visual Studio 2017 with C# installed to open and compile the projects. All projects have their MonoGame-based dependencies linked to NuGet packages, so you should be able to build the project and run any one of the samples from the get-go.

## Updates

Version 1.2 adds bounding rectangles. These have two modes, boxes and rotation-less squares, each mode being useful for a different application. Samples now draw these rectangles over the objects.

Version 1.1 adds the choice of either collision detection or collision resolution. Detection is faster, but resolution produces a Minimum Translation Vector that can be used in physics.

Version 1.0 allows for accurate collision detection between any two of polygons or circles, as well as quick projection-based collision resolution.

## Quick Use Guide

IPolygons are either circles or freeform (convex) polygons. Shapes are readonly structures inside Polygons that maintain shape data.

To perform simple collision detection, call CheckCollision(...) from one of your IPolygons. To perform simple collision detection and response, call CheckCollisionAndRespond(...) from one of your IPolygons.

Bounding boxes are also provided. You can use GetBoundingBox() or GetBoundingSquare() to generate a bounding rectangle for your IPolygon. You can then call CollidesWith(...) from one of these bounding rectangles to perform a basic broad-phase check prior to using the accurate collision detection.

## To-Do

- Collision World object to handle broad-phase through a spatial hash or a quadtree.

- More options to tweak and optimize the system.


