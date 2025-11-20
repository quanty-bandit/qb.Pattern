using qb.Pattern;
namespace qb.Test
{
    public class MonoSingletonSampleExclusive : MonoSingletonSampleExtention
    {
        public override EDuplicatedSingletonInstanceAction DuplicatedInstanceAction => EDuplicatedSingletonInstanceAction.Exception;
    }
}
