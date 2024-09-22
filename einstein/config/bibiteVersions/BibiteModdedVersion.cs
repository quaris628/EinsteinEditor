using Einstein.config.bibiteVersions.vanilla;
using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config.bibiteVersions
{
    public abstract class BibiteModdedVersion : BibiteVersion
    {
        private BibiteVanillaVersion baseVanillaVersion;

        public BibiteModdedVersion(BibiteVanillaVersion baseVanillaVersion)
        {
            this.baseVanillaVersion = baseVanillaVersion;
        }

        public BibiteModdedVersion(BibiteVersion baseVanillaVersion)
        {
            if (!(baseVanillaVersion is BibiteVanillaVersion baseVanillaVersion1))
            {
                throw new InvalidOperationException("baseVanillaVersion must be vanilla");
            }
            this.baseVanillaVersion = baseVanillaVersion1;
        }

        public override BibiteVanillaVersion GetVanilla()
        {
            return baseVanillaVersion;
        }

        internal abstract BaseBrain CreateVanillaCopyOf(BaseBrain brain);

        internal abstract BaseBrain CreateCopyFromVanilla(BaseBrain brain);
    }
}
