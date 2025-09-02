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
			Icon = ModEntry.Instance.PatientIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "description"])
		};
	
	public bool GetIsPatient(Card card)
		=> card.GetIsPatient();

	public void SetIsPatient(Card card, bool value)
		=> card.SetIsPatient(value);

	public IStatusEntry FrazzleStatus => ModEntry.Instance.FrazzleStatus;
	public IStatusEntry RebuttalStatus => ModEntry.Instance.RebuttalStatus;
	public IStatusEntry SuperBoostStatus => ModEntry.Instance.SuperBoostStatus;
	public IStatusEntry SympathyStatus => ModEntry.Instance.SympathyStatus;
	public IStatusEntry SilenceStatus => ModEntry.Instance.SilenceStatus;
	public Tooltip GetFrazzleTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Frazzle")
		{
			Icon = ModEntry.Instance.FrazzleIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["status", "Frazzle", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["status", "Frazzle", "description"])
		};
	public Tooltip GetRebuttalTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Rebuttal")
		{
			Icon = ModEntry.Instance.RebuttalIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["status", "Rebuttal", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["status", "Rebuttal", "description"])
		};
	public Tooltip GetSuperBoostTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::SuperBoost")
		{
			Icon = ModEntry.Instance.SuperBoostIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["status", "SuperBoost", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["status", "SuperBoost", "description"])
		};
	public Tooltip GetSympathyTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Sympathy")
		{
			Icon = ModEntry.Instance.SympathyIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["status", "Sympathy", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["status", "Sympathy", "description"])
		};
	public Tooltip GetSilenceTooltip()
		=> new GlossaryTooltip($"cardtrait.{ModEntry.Instance.Package.Manifest.UniqueName}::Silence")
		{
			Icon = ModEntry.Instance.SilenceIcon.Sprite,
			TitleColor = Colors.cardtrait,
			Title = ModEntry.Instance.Localizations.Localize(["status", "Silence", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["status", "Silence", "description"])
		};
}
