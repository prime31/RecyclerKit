RecyclerKit
===========

RecyclerKit is an object pooling system that is super easy to use. Stick a TrashMan component on any GameObject in your scene and drag any prefabs you want pooled into it's inspector. You can configure the total objects to preallocate and a couple other things there. Everything has tooltips so just hover your mouse over them to see what they do. You then use the spawn method where you would normally call Instantiate and the despawn method where you would normally call Destroy.


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

