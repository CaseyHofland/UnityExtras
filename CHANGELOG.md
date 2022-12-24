# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2022-12-24
### Added
- Add IKPlanter, a component that uses the Animation Rigging package to e.g. plant feet on the ground, or hands on objects.
- Add FocusPointConstraint and FocusPoint, which together with the Animation Rigging package create a system for having the character look at objects in your world.
- Add LinkPropertyAttribute, an attribute that allows you to link a field to a property. See 82134a1d97277d5010a39289bc09be8edc3d0004 for an example of how to use it.
- Add CenterOfMass, a component that allows you to offset a Rigidbodies center of mass.
- Add PointEffector, a 3D equivalent of PointEffector2D.
- Add SurfaceEffector, a 2D equivalent of SurfaceEffector2D.
- Add PropertyMember for Editor functionality. SerializedProperty.GetPropertyMember returns a PropertyMember that holds the reflected member and target of the property, as well as its parents, allowing you to set its value or get its type using reflection.

### Removed
- SerializedPropertyExtensions.GetMember, -.GetField, -.GetProperty, -.GetValue, -.SetValue, -.GetSanitizedPropertyType have all been removed in favor of the new PropertyMember class.

### Fixed
- ValueRef now inherits from UnityMember to fix a few inspector issues, as well as standardize its implementation.

## [1.1.0] - 2022-10-06
### Added
- Add a Reset item to the contextual property menu. This allows you to reset properties by right-clicking and selecting "Reset".
- Add an Object Picker to the Object Drawer. Object Drawers now contain a button that, when selected, allow you to click an object in the scene in order to select is for the object field.
- Add null checks to the FirstPersonCharacter and ThirdPersonCharacter2D.
- Add null check in DestroySafe.
- Add DestroyImmediateSafe to ExtraObject methods.
- Add Layer struct. Layer can be used in much the same way you would use LayerMask. The difference is that Layer only allows the selection of a single layer, and its value represents an index. This means that Layer.value might be 8 where LayerMask.value would be 256 (1 << 8).
- Add a RequiredComponent.GetComponent(GameObject, HideFlags) override. This allows to assign HideFlags on object creation. Useful for setting the HideFlags to HideFlags.HideInInspector on creation, a common use case.
- Add low hanging fruit MethodImpl(MethodImplOptions.AggressiveInlining) attribute to ExtraMath operations.
- Add IAuthor, an interface that, together with RequiredComponent<T>, provides an effective way to Author components from your custom scripts. CharacterController2D and SliderJoint implement this to cleanly Author a CapsuleCollider2D and ConfigurableJoint respectively. It is crucial that IAuthor be implemented properly, thus a code example has been provided in the comments of IAuthor (that is, the box you see when hovering over it in your favorite compiler). Use this as a starting point when in doubt about its proper implementation.
- Add InputReaction, a format in which you may assign an Input, Processors and a ReactionMethod in order to work with input safely and efficiently.
- Add utility script for finding missing scripts in a scene. Can be found under "Window/Find Missing Scripts".
- Add a LocalPositionSwitch. Switch is an abstract class that will trigger UnityEvents when the switch is turned on or off based on a condition. LocalPositionSwitch is a switch that triggers when an object reaches a target local position. This format allows for quickly setting up gameplay through UnityEvent, and to easily add more switches by inheriting from Switch.
- Add UnityMember and ValueRef<>. Unity Member is a serializable MemberInfo. It is modelled after UnityEvent and amazingly powerful for linking values in-editor. ValueRef<> is a wrapper for UnityMember to allow for links between value references, meaning fields and read-writeable properties. This can be used in an inspector that needs to manipulate some kind of value, but doesn't know up front what value that is going to be.
- Add UnityListDefaultsAttribute. Serialized lists in Unity don't reset new classes to their defaults, which causes issues for UnityMember. By decorating a serialized list with the UnityListDefaultsAttribute, this behavior is fixed.
- Add Picker2D and PickUp2D for picking up objects in 2D.
- Add ValueStore<>, a wrapper that allows for a value to be stored and replaced temporarily. This is useful in PickUp as you want a rigidbody to have different settings during its PickUp state (e.g. gravity = false) but reset this value once you get out of this state.

### Changed
- Update minimum Unity version to 2021.3.
- UnityExtras.OnEvent has been renamed to UnityExtras.Events.MonoEvent. It functions the same.

### Removed
- Remove ExtraMath.InverseSafe. Use `math.select(math.rcp(myFloat), 0f, myFloat == 0f);` instead.

### Fixed
- Fix Line Endings in ContinuousInteraction.
- The rigidbody context menu "Use 2D Drag" didn't correctly change the angular drag. This has been fixed. Additionally, the operation is recorded on the Undo stack.
- Fix for DestroySafe failing inside the editor when the application is quitting.
- SliderJointEditor had an incorrect handle size gizmo.
- SliderJoint would log forward being zero upon conversion: now a check is in place to make sure forward is not zero.
- Add Vector3.zero check before calling Quaternion.LookRotation in CharacterController2D.
- Fix PickUp gyroTarget edge case. When the Pickers forward rotation was equal to Vector3.up and the PickUp.followUpwards was set to 0, the gyro target would incorrectly be set to Quaternion.identity. This has been fixed and now the PickUp will follow the same y rotation as the Picker.

## [1.0.0] - 2022-05-20
### Added
- InputSystem 1.3.0 as a dependancy.
- A ContinuousInteraction for the InputSystem. This interaction will continuously trigger the "performed" event as long as the action is being performed.
- Coyote time to the CharacterMover: if you walk off the ground, you still have a little time to jump.
- Jump buffer to the CharacterMover: if you jump before hitting the ground, the jump will still trigger if you hit the ground within the aloted time.
- Peak time to the CharacterMover: specify how long it takes to reach the peak of your jump.
- Fast fall ratio to the CharacterMover: if you stop jumping while not having reached your peak you will fall more quickly. This enables variable jump height.
- Added Input handling to FirstPersonCharacter.
- RigidbodySettings to the Pick Up script. A Pick Up can now specify how it should override its rigibody when it gets picked up. This may be used to e.g. disable gravity when the body gets picked up.
- Picker gizmos showing its picking range, as well as if there's a Pick Up in range.
- Picker can now take optional input.
- The NonResetable<> struct. It elegantly circumvents a value from being Reset by the Component > Reset menu item. Simply, if you have a NonResetable<float> f that is equal to 20, then reset, f remains equal to 20.
- The recreated SliderJoint. It is a prismatic joint build from a Configurable Joint under the hood, that may slide across a single axis from a minimum to a maximum point. This can be used to e.g. create sliding doors.
- The SliderJointAnchorEditorTool, which allows you to edit the anchors on a Slider Joint.
- The SliderJointAngleEditorTool, which allows you to edit the angle of a Slider Joint.
- The SliderJointLimitsEditorTool, which allows you to edit the limits of a Slider Joint.
- A max release force to Pick Up. This allows you to specify the maximum speed with which a pick up will fly after it's been released.
- Icons for InitialForce and InitialForce2D.
- Meta files for the SliderJoint icons.
- ExtraGizmos, with helper methods to draw Gizmos (for example DrawWireCapsule).
- ExtraMath.Rotate2D, which rotates a Vector2 by a specified angle.
- The RequiredComponent<Component> struct. This struct ensures a component to be present when requested, in a manner that handles Component Reset, Undo operations and entering and exiting playmode. It has been used to Bind a Configurable Joint to the Slider Joint, as well as a CapsuleCollider2D to the CharacterController2D.
- The Input struct. It is a convenient type that allows you to add input in such a way that you can always change your setup later without changing your code. It consists of:
    - An InputActionProperty, which allows you to choose between either an InputActionReference or a custom constructed InputAction.
    - A bool to specify if you want to enable the action on start or not.
    - A foldout to hide the action for a sleeker inspector.

    The intended use case for Input is that you can start off with a custom InputAction and the enableOnStart toggle set to true. This allows you to test your input immediately.
    Once you're satisfied, you may change to an InputActionReference from a dedicated InputActionAsset.
    If later you implement ways for different input to be turned on or off, you may disable the enableOnStart toggle and let your own code take over.
- A CharacterController2D.
    The CharacterController2D is a marvel: it replicates as best it can Unity's own CharacterController. While not perfect (e.g. horizontal overlap recovery is not correctly implemented and currently always disabled) the CharacterController2D moves near perfect in tandem with the CharacterController.

    Additionally, all CharacterController extension components have been recreated for 2D as well.
    - The CharacterController2D sends ControllerColliderHit2Ds out which can be received by methods named "OnControllerColliderHit2D".
    - The CharacterPusher2D uses this to push objects the CharacterController2D collides with, much like the CharacterPusher does.
    - The CharacterMover2D allows easy customization of your character's movement and provides all the same methods as the CharacterMover. Note however that CharacterMover2D.Turn() simply takes a bool if it should be facing to the right or not.
    - The ThirdPersonCharacter2D script takes input for controlling the CharacterController2D.

    The CharacterController2D has been given the same icon as the CharacterController, much like Rigidbody and Rigidbody2D have the same icon.
- UnityEventExtensions. GetPersistentEventIndex can be used to find the index of a persistent listener.

### Changed
- Renamed CharacterMover.sprintSpeed to CharacterMover.sprintBoost, to reflect that sprintBoost is added ON TOP OF CharacterMover.moveSpeed.
- Renamed ExtraMath.Vector2ToDegree to Angle.
- Renamed ExtraMath.DegreeToVector2 to Direction.

### Removed
- The redundant InvalidConfigurationException.
- ExtraMath.Vector2ToRadian.
- ExtraMath.RadianToVector2.

## [0.0.1-exp] - 2022-04-26
Initial Commit.