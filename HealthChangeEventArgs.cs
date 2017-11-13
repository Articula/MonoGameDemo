using System;
namespace MonoGameDemo
{
	public class HealthChangeEventArgs : EventArgs
	{
		public HealthChangeEventArgs(bool isPositive, int value)
		{
			this.isPositive = isPositive;
			this.value = value;
		}

		public bool isPositive { get; }
		public int value { get; }
	}
}
