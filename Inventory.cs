using System;
namespace MonoGameDemo
{
	public class Inventory
	{
		public int gemCount { get; set; }

		public Inventory()
		{
			gemCount = 0;
		}

		public void add(ICollectable item) 
		{
			switch (item.itemId) {
				case ItemId.Gem:
					gemCount++;
					break;
			}
		}
	}
}
