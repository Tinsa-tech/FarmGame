public class Harvester : Worker
{
    public override void doJob(Field doJobOn)
    {
        doJobOn.Harvest();
    }
}
