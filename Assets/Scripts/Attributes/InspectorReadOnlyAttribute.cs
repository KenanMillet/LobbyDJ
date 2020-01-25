namespace StackableDecorator
{
	public class DisableAttribute : EnableIfAttribute
	{
		public DisableAttribute() : base(false) { }
	}
}