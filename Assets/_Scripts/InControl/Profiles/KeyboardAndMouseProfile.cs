using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace CustomProfileExample
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager component, or you can attach it yourself like so:
	// InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
	// 
	public class KeyboardAndMouseProfile : UnityInputDeviceProfile
	{
		public KeyboardAndMouseProfile()
		{
			Name = "Keyboard/Mouse";
			Meta = "A keyboard and mouse combination profile appropriate for FPS.";

			// This profile only works on desktops.
			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;
			UpperDeadZone = 1.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Jump",
					Target = InputControlType.Action1,
					Source = KeyCodeButton( KeyCode.W )
				},
				new InputControlMapping
				{
					Handle = "Teleport",
					Target = InputControlType.Action3,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Hook",
					Target = InputControlType.Action4,
					Source = KeyCodeButton( KeyCode.O )
				},
				new InputControlMapping
				{
					Handle = "Power_2",
					Target = InputControlType.RightBumper,
					Source = MouseButton1
				},
				new InputControlMapping
				{
					Handle = "LowFPS",
					Target = InputControlType.Action2,
					Source = KeyCodeButton( KeyCode.P )
				},
				new InputControlMapping
				{
					Handle = "StartCommand",
					Target = InputControlType.Menu,
					Source = KeyCodeButton( KeyCode.Escape )
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move X",
					Target = InputControlType.LeftStickX,
					// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D ),
					Raw = true
				},
				new InputControlMapping
				{
					Handle = "Move Y",
					Target = InputControlType.LeftStickY,
					// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W ),
					Raw = true
				},
				new InputControlMapping {
					Handle = "Move X Alternate",
					Target = InputControlType.LeftStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping {
					Handle = "Move Y Alternate",
					Target = InputControlType.LeftStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				},
				new InputControlMapping
				{
					Handle = "Look X",
					Target = InputControlType.RightStickX,
					Source = MouseXAxis,
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Look Y",
					Target = InputControlType.RightStickY,
					Source = MouseYAxis,
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Look Z",
					Target = InputControlType.ScrollWheel,
					Source = MouseScrollWheel,
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Toggle Texture Paint",
					Target = InputControlType.LeftTrigger,
					Source = KeyCodeButton( KeyCode.Q ),
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Power_1",
					Target = InputControlType.RightTrigger,
					Source = MouseButton0,
					Raw    = true,
					Scale  = 0.1f
				}
			};
		}
	}
}

