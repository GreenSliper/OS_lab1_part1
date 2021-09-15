using System;
using System.Linq;

namespace OS_lab1
{
	public interface IMenuItem
	{
		void Select(IMenu invokedFrom = null);
		void PrintItem();
	}

	public interface IMenu : IMenuItem
	{
		IMenu GetPreviousMenu();
	}

	public interface IYNmessageBox
	{
		bool Show(string header);
	}

	public class YNMessageBox : IYNmessageBox
	{
		public bool Show(string header)
		{
			bool success = false;
			do
			{
				Console.WriteLine(header);
				Console.WriteLine("Y/N? ");
				string res = Console.ReadLine().ToLower();
				if (res == "y" || res == "yes")
					return true;
				if (res == "n" || res == "no")
					return false;
			} while (!success);
			return false;
		}
	}

	public class MenuItem : IMenuItem
	{
		protected string itemText;
		Action action;
		public MenuItem(string itemText, Action action)
		{
			this.itemText = itemText;
			this.action = action;
		}

		public void PrintItem()
		{
			Console.WriteLine(itemText);
		}

		public void Select(IMenu invokedFrom)
		{
			action?.Invoke();
			Console.ReadKey();
			invokedFrom.Select(invokedFrom.GetPreviousMenu());
		}
	}
	public class Menu : IMenu
	{
		protected string menuHeader;
		protected IMenuItem[] menuItems;
		public IMenu PreviousMenu { get; private set; }
		public IMenu GetPreviousMenu() => PreviousMenu;

		readonly string selectAgainText = "Incorrect input. Try again.",
			selectText = "Please select option using number in range [FROM, TO]:",
			toPrevMenuText = "Return to previous menu",
			exitText = "Exit";

		protected Action onFirstSelectAction, onMenuLeaveAction;
		bool onFirstSelectActionInvoked = false;

		public Menu(string menuHeader, IMenuItem[] menuItems)
		{
			this.menuHeader = menuHeader;
			this.menuItems = menuItems;
		}

		public Menu(string menuHeader, IMenuItem[] menuItems, 
			string selectAgainText, string selectText, string toPrevMenuText, string exitText) : this(menuHeader, menuItems)
		{
			this.selectAgainText = selectAgainText;
			this.selectText = selectText;
			this.toPrevMenuText = toPrevMenuText;
			this.exitText = exitText;
		}

		public Menu AddOnFirstSelectAction(Action act)
		{
			onFirstSelectAction += act;
			return this;
		}

		public Menu RemoveOnFirstSelectAction(Action act)
		{
			onFirstSelectAction -= act;
			return this;
		}

		public void PrintItem()
		{
			Console.WriteLine(menuHeader);
		}

		protected virtual void PrintMenu()
		{
			Console.Clear();
			Console.WriteLine(menuHeader);
			Console.WriteLine();
			int counter = 1;
			foreach (var it in menuItems)
			{
				Console.Write(counter++ + "). ");
				it.PrintItem();
			}
			Console.WriteLine();
			Console.WriteLine("0). " + (PreviousMenu==null ? exitText : toPrevMenuText));
			Console.WriteLine();
			Console.WriteLine(selectText.Replace("FROM", "1")
				.Replace("TO", menuItems.Length.ToString()));
		}

		int GetUserSelectedItemIndex()
		{
			int selectedIndex = -1;
			do
			{
				if(!int.TryParse(Console.ReadLine(), out selectedIndex)
					|| selectedIndex < 0 || selectedIndex > menuItems.Length)
				{
					selectedIndex = -1;
					Console.WriteLine(selectAgainText);
					Console.ReadKey();
					PrintMenu();
				}
			} while (selectedIndex < 0);
			return selectedIndex;
		}

		public virtual void Select(IMenu invokedFrom = null)
		{
			if (!onFirstSelectActionInvoked)
			{
				onFirstSelectAction?.Invoke();
				onFirstSelectActionInvoked = true;
			}
			if (invokedFrom != null)
				PreviousMenu = invokedFrom;
			PrintMenu();
			int selection = GetUserSelectedItemIndex();
			if (selection != 0)
				menuItems[selection - 1]?.Select(this);
			else
			{
				onMenuLeaveAction?.Invoke();
				PreviousMenu?.Select(null);
			}
		}
	}

	public class FlexMenu : Menu
	{
		public FlexMenu(string menuHeader, IMenuItem[] menuItems) : base(menuHeader, menuItems) { }

		public FlexMenu(string menuHeader, IMenuItem[] menuItems,
			string selectAgainText, string selectText, string toPrevMenuText, string exitText)
			: base(menuHeader, menuItems, selectAgainText, selectText, toPrevMenuText, exitText) { }

		public void AddItems(params IMenuItem[] items)
		{
			Array.Resize(ref menuItems, menuItems.Length + items.Length);
			Array.Copy(items, 0, menuItems, menuItems.Length - items.Length, items.Length);
		}
	}

	public class MenuWithDataRequest<T> : Menu
	{
		bool dataSet = false;
		public T Data { get; protected set; }
		protected Func<T> dataRequester;
		public MenuWithDataRequest(string menuHeader, IMenuItem[] menuItems, Func<T> dataRequester) : base(menuHeader, menuItems) 
		{
			this.dataRequester = dataRequester;
			onMenuLeaveAction += () => dataSet = false;
		}
		public MenuWithDataRequest(string menuHeader, IMenuItem[] menuItems, Func<T> dataRequester,
			string selectAgainText, string selectText, string toPrevMenuText, string exitText)
			: base(menuHeader, menuItems, selectAgainText, selectText, toPrevMenuText, exitText)
		{
			this.dataRequester = dataRequester;
			onMenuLeaveAction += () => dataSet = false;
		}

		public override void Select(IMenu invokedFrom = null)
		{
			if (!dataSet)
			{
				dataSet = true;
				Data = dataRequester.Invoke();
			}
			base.Select(invokedFrom);
		}
		protected override void PrintMenu()
		{
			base.PrintMenu();
			Console.WriteLine("Inspected item: " + Data.ToString());
		}
	}

}
