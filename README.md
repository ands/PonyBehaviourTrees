PonyBehaviourTrees (PBT)
========================

PBT is a set of libraries to create, execute and inspect behaviour trees.
These are often used to create artificial intelligences for games.
The possibility of editing the PBTs while the application is running,
C# scripting inside nodes and runtime inspection makes this a flexible and powerful tool.

![PBT Editor](http://andsz.de/i/20140622030049456.png)
![PBT Inspector](http://andsz.de/i/20140622030929222.png)


Resources
=========

- http://aigamedev.com/open/article/bt-overview/
- http://www.altdev.co/2011/02/24/introduction-to-behavior-trees/
- http://magicscrollsofcode.blogspot.de/2010/12/behavior-trees-by-example-ai-in-android.html
- http://chrishecker.com/My_Liner_Notes_for_Spore/Spore_Behavior_Tree_Docs
- http://twvideo01.ubm-us.net/o1/vault/gdc10/slides/ChampandardDaweHernandezCerpa_BehaviorTrees.pdf
- http://docs.cryengine.com/display/SDKDOC4/Coordinating+Agents+with+Behavior+Trees
- http://en.wikipedia.org/wiki/Behavior_Trees


TODO
====

- Write: "Getting Started"
- Write: "How to implement my own Tasks"


Known Issues
============

- OpenGL context switching is used incorrectly somewhere. This is sometimes breaking the fonts if multiple inspector/editor windows were opened.
- If a script can not be compiled anymore (during program start), it will throw an exception on each PBT instance that is using it. Not sure how to solve this...