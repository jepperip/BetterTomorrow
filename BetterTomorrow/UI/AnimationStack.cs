using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.Graphics;

namespace BetterTomorrow.UI
{
	public interface IAnimatable
	{
		List<ValueAnimator> Animators { get; }
	}

	class TextViewColorAnimator : IAnimatable
	{
		public TextViewColorAnimator(
			TextView textView,
			int from,
			int to,
			int duration)
		{
			textView.SetTextColor(new Color(from));
			var animator = ValueAnimator.OfArgb(from, to);
			animator.SetDuration(duration);
			animator.Update += (sender, e) =>
			{
				textView.SetTextColor(new Color((int)e.Animation.AnimatedValue));
			};

			Animators.Add(animator);
		}

		public List<ValueAnimator> Animators { get; private set; } = new List<ValueAnimator>();
	}

    class ViewFadeAnimator : IAnimatable
    {
        public ViewFadeAnimator(
            View imageView,
            int duration,
            float from = 0.0f,
            float to = 1.0f)
        {
            imageView.Alpha = from;
            var animator = ValueAnimator.OfFloat(from, to);
            animator.SetDuration(duration);
            animator.Update += (sender, e) =>
            {
                imageView.Alpha = (float)e.Animation.AnimatedValue;
            };
            Animators.Add(animator);
        }

        public List<ValueAnimator> Animators { get; } = new List<ValueAnimator>();
    }

	class ViewPositionAnimator : IAnimatable
	{
		public ViewPositionAnimator(
			View view,
			float fromX,
			float toX,
			float fromY,
			float toY,
			int duration)
		{
			view.SetX(fromX);
			view.SetY(fromY);
			var xAnimator = ValueAnimator.OfFloat(fromX, toX);
			xAnimator.SetDuration(duration);
			xAnimator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				view.SetX((float)e.Animation.AnimatedValue);
			};

			var yAnimator = ValueAnimator.OfFloat(fromY, toY);
			yAnimator.SetDuration(duration);
			yAnimator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				view.SetY((float)e.Animation.AnimatedValue);
			};

			Animators.Add(xAnimator);
			Animators.Add(yAnimator);
		}
		public List<ValueAnimator> Animators { get; } = new List<ValueAnimator>();
	}

	public class AnimationStack
	{
		private Stack<ValueAnimator> animations =
			new Stack<ValueAnimator>();

		private int delay;


		public void PushAnimation(IAnimatable animation)
		{
			foreach (var animator in animation.Animators)
			{
				animator.StartDelay = delay;
				animations.Push(animator);
			}
		}

		public void AddDelay(int ms)
		{
			delay += ms;
		}

		public void Start()
		{
			foreach (var animation in animations)
			{
				animation.Start();
			}
		}

		public void Clear()
		{
			animations.Clear();
			delay = 0;
		}
	}
}