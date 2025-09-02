using Nickel;

namespace Flipbop.Rosa;

public interface IRosaApi
{
	IDeckEntry RosaDeck { get; }

	ICardTraitEntry PatientTrait { get; }
	Tooltip GetPatientTooltip();
	bool GetIsPatient(Card card);
	void SetIsPatient(Card card, bool value);
	IStatusEntry FrazzleStatus { get; }
	IStatusEntry RebuttalStatus { get; }
	IStatusEntry SuperBoostStatus { get; }
	IStatusEntry SympathyStatus { get; }
	IStatusEntry SilenceStatus { get; }
	Tooltip GetFrazzleTooltip();
	Tooltip GetRebuttalTooltip();
	Tooltip GetSuperBoostTooltip();
	Tooltip GetSympathyTooltip();
	Tooltip GetSilenceTooltip();
}
