using Nickel;

namespace Flipbop.Rosa;

public sealed class ApiImplementation : IRosaApi
{
	public IDeckEntry RosaDeck
		=> ModEntry.Instance.RosaDeck;

	public ICardTraitEntry PatientTrait
		=> PatientManager.Trait;

	public Tooltip GetPatientTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Patient")
		{
			Icon = ModEntry.Instance.FrazzleIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "description"])
		};
	
	public bool GetIsPatient(Card card)
		=> card.GetIsPatient();

	public void SetIsPatient(Card card, bool value)
		=> card.SetIsPatient(value);
	
}
