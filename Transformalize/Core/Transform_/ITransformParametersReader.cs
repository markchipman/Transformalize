using Transformalize.Configuration;
using Transformalize.Core.Parameters_;

namespace Transformalize.Core.Transform_
{
    public interface ITransformParametersReader
    {
        Parameters Read(TransformConfigurationElement transform);
    }
}