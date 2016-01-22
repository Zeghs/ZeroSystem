using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Products;

namespace Taiwan.Rules.Contracts {
	internal interface IContractParameters {
		void SetParameters(List<SessionObject> sessions);
	}
}