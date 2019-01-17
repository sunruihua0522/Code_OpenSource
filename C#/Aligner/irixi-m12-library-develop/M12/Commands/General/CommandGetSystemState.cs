namespace M12.Commands.General
{
    public class CommandGetSystemState : CommandBase
    {
        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_GET_SYS_STA;
            }
        }

        #endregion

    }
}
