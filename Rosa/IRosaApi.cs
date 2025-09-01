using Nickel;

namespace Flipbop.Rosa;

public interface IRosaApi
{
	IDeckEntry RosaDeck { get; }

	ICardTraitEntry PatientTrait { get; }
	Tooltip GetPatientTooltip();
	bool GetIsPatient(Card card);
	void SetIsPatient(Card card, bool value);
}
