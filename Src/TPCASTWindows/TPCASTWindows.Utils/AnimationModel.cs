using System;
using System.Windows.Forms;

namespace TPCASTWindows.Utils
{
	internal class AnimationModel
	{
		public delegate void OnConnectAnimateionPauseDelegate();

		public delegate void OnConnectAnimateionResumeDelegate();

		private static Control sContext;

		public static AnimationModel.OnConnectAnimateionPauseDelegate OnConnectAnimateionPause;

		public static AnimationModel.OnConnectAnimateionResumeDelegate OnConnectAnimateionResume;

		public static void init(Control context)
		{
			AnimationModel.sContext = context;
		}

		public static void setAnimationCallback(AnimationCallback animationCallback)
		{
			if (animationCallback != null)
			{
				AnimationModel.OnConnectAnimateionPause = new AnimationModel.OnConnectAnimateionPauseDelegate(animationCallback.OnConnectAnimateionPause);
				AnimationModel.OnConnectAnimateionResume = new AnimationModel.OnConnectAnimateionResumeDelegate(animationCallback.OnConnectAnimateionResume);
			}
		}

		public static void ConnectAnimateionPause()
		{
			if (AnimationModel.sContext != null && AnimationModel.OnConnectAnimateionPause != null)
			{
				if (AnimationModel.sContext.InvokeRequired)
				{
					AnimationModel.sContext.Invoke(AnimationModel.OnConnectAnimateionPause);
					return;
				}
				AnimationModel.OnConnectAnimateionPause();
			}
		}

		public static void ConnectAnimateionResume()
		{
			if (AnimationModel.sContext != null && AnimationModel.OnConnectAnimateionResume != null)
			{
				if (AnimationModel.sContext.InvokeRequired)
				{
					AnimationModel.sContext.Invoke(AnimationModel.OnConnectAnimateionResume);
					return;
				}
				AnimationModel.OnConnectAnimateionResume();
			}
		}
	}
}
