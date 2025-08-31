using Nickel;

namespace Flipbop.Cleo;

public sealed class ApiImplementation : ICleoApi
{
	public IDeckEntry CleoDeck
		=> ModEntry.Instance.CleoDeck;

	public ICardTraitEntry ImprovedACardTrait
		=> ImprovedAManager.Trait;
	public ICardTraitEntry ImprovedBCardTrait
		=> ImprovedBManager.Trait;
	
	public ICardTraitEntry ImpairedTrait
		=> ImpairedManager.Trait;

	public Tooltip GetImprovedATooltip(bool amount)
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::ImprovedA")
		{
			Icon = ModEntry.Instance.ImprovedIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "ImprovedA", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "ImprovedA", "description"])
		};
	
	public Tooltip GetImprovedBTooltip(bool amount)
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::ImprovedB")
		{
			Icon = ModEntry.Instance.ImprovedIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "ImprovedB", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "ImprovedB", "description"])
		};
	public Tooltip GetImpairedTooltip(bool amount)
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Impaired")
		{
			Icon = ModEntry.Instance.ImprovedIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "Impaired", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "Impaired", "description"])
		};
	
	public bool GetIsImprovedA(Card card)
		=> card.GetIsImprovedA();

	public void SetIsImprovedA(Card card, bool value)
		=> card.SetIsImprovedA(value);

	public void AddImprovedA(Card card, State s)
		=> card.AddImprovedA(s);
	
	public bool GetIsImprovedB(Card card)
		=> card.GetIsImprovedB();

	public void SetIsImprovedB(Card card, bool value)
		=> card.SetIsImprovedB(value);

	public void AddImprovedB(Card card, State s)
		=> card.AddImprovedB(s);
	public bool GetIsImpaired(Card card)
		=> card.GetIsImpaired();

	public void SetIsImpaired(Card card, bool value)
		=> card.SetIsImpaired(value);

	public void AddImpaired(Card card, State s)
		=> card.AddImprovedB(s);

}
