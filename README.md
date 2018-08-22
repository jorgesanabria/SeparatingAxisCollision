# SeparatingAxisCollision
## simple 2D SAT-based collision detection and response library for .NET

You will need Visual Studio 2017 with .NET Framework and .NET Core SDK installed to open and compile the projects. The CollisionTester app has its MonoGame dependency linked via NuGet, so with an IDE you should be able to build and run from the get-go.

## Updates

Version 2.3 adds four new methods to IPolygon, Clone, Translate, Rotate, AddScale, and MultiplyScale. No more pesky SetRotation(GetRotation()) combos! Also fixes a slight possible bug with position setting in Box.

Version 2.2.1 fixes a bug where Box's points would not update on rotation in certain situations and also renames the solution to the proper name.

Version 2.2 changes the project types to multitarget .NET Framework 4.5 and .NET Standard 1.0. SeparatingAxisCollision.polygons was renamed to SeparatingAxisCollision.Primitives. Functionality remains identical to version 2.1.

Version 2.1 bases all calculations on Doubles instead of Singles now. It adds in Starry.Math, a Unity-esque double-precision floating point math library that handles all the math code used in SeparatingAxisCollision. SeparatingAxisCollision no longer depends on MonoGame or System.Drawing! The CollisionTester's colliding colors have now changed. See SampleUse.txt for more info!

Version 2.0 rehauls the project for large improvements. Collision queries are now done through the static Collision class. Two new primitives, Box and Ray, are added. There is now only one sample project that is able to check collision between any two types of IPolygon primitive. Unnecessary drawing code has been moved to the sample project, to indicate that I will not be maintaining/optimizing such code.

Version 1.2 adds bounding rectangles. These have two modes, boxes and rotation-less squares, each mode being useful for a different application. Samples now draw these rectangles over the objects.

Version 1.1 adds the choice of either collision detection or collision resolution. Detection is faster, but resolution produces a Minimum Translation Vector that can be used in physics.

Version 1.0 allows for accurate collision detection between any two of polygons or circles, as well as quick projection-based collision resolution.

## Quick Use Guide

Collision is checked between primitives, which are all accessible via the IPolygon interface.

There are four types: Box, Circle, Freeform Convex, and Ray

Freeform Convex can technically replace the functionality of Box or Ray, but Box and Ray are more optimized for their shapes.

Collision queries are carried out from the static Collision class. CheckCollision(...) checks if two primitives are colliding. CheckCollisionAndRespond(...) allows you to gain a MTV (Minimum Translation Vector) for projection-based collision response.

Bounding boxes are also provided. You can use GetBoundingBox() or GetBoundingSquare() to generate a bounding rectangle for your IPolygon. You can then call CollidesWith(...) from one of these bounding rectangles to perform a basic broad-phase check prior to using the accurate collision detection.

## To-Do

- Broadphase system utilizing a spatial hash or a quadtree to optimize away unneeded SAT checks in 2D space.
