public enum camera_mode
{
	FOLLOWING,
	ON_RAILS
};

//Class that includes all the camera changes information
[System.Serializable]
public class CameraBehaviour {
	
	public camera_mode mode = camera_mode.FOLLOWING;
	public float upPivot;
	public float downPivot;

}