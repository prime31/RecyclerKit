RecyclerKit
===========

RecyclerKit is an object pooling system that is super easy to use. Stick a TrashMan compoenent on any GameObject in your scene and drag any prefabs you want pooled into it's inspector. You can configure the total objects to preallocate and a couple other things there. Everything has tooltips so just hover your mouse over them to see what they do. You then use the spawn method where you would normally call Instantiate and the despawn method where you would normally call Destroy.


Usage
----
- drag the TrashMan script onto a GameObject in your scene (it is usually a good idea to use the Script Execution Order to have TrashMan run before other scripts)
- in the TrashMan inspector, drag any prefabs (or GameObjects from the current scene) that you want it to manage and setup the recycle bin details
- use the following API to manage instances:
	* TrashMan.spawn replaces your calls to Instantiate
	* TrashMan.despawn or TrashMan.despawnAfterDelay replaces your calls to Destroy
	* TrashMan.manageRecycleBin creates a new recycle bin at runtime in case you don't want to use the Editor


Optional Branch
----

The ObjectComponent branch has one minor change: any recycleable object must have the TrashManRecycleableObject component on it. This does enable a couple niceties such as spawn/despawn events and no need to access GameObject.name (which allocates a bit of memory)


License
-----
[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US). You are free to use RecyclerKit in any and all games that you make. You cannot sell RecyclerKit directly or as part of a larger game asset.



Attribution
-----

I would like to give attribution to the original inspiration for RecyclerKit but I have no clue who or where the first iteration came from. At this point, RecyclerKit is so far removed from its original inspiration that it is impossible to trace its origin.
