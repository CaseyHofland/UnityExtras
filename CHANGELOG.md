# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0-exp] - 2022-05-20
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