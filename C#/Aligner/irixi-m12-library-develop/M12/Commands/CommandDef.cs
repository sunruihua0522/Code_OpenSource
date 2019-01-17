namespace M12.Commands
{
    /// <summary>
    /// The definition of commands supported by the bootloader.
    /// </summary>
    public enum Commands
    {
        HOST_CMD_CHECKMODE,
        HOST_CMD_HOME,
        HOST_CMD_MOVE,
        HOST_CMD_MOVE_T_OUT,
        HOST_CMD_MOVE_T_ADC,
        HOST_CMD_STOP,
        HOST_CMD_SET_ACC,
        HOST_CMD_SET_MODE,
        HOST_CMD_GET_SYS_INFO,
        HOST_CMD_GET_MCSU_STA,
        HOST_CMD_GET_MCSU_SETTINGS,
        HOST_CMD_GET_SYS_STA,
        HOST_CMD_GET_ERR,
        HOST_CMD_GET_MEM_LEN,
        HOST_CMD_READ_MEM,
        HOST_CMD_CLEAR_MEM,
        HOST_CMD_SET_DOUT,
        HOST_CMD_READ_DOUT,
        HOST_CMD_READ_DIN,
        HOST_CMD_READ_AD,
        HOST_CMD_EN_CSS,
        HOST_CMD_SET_CSSTHD,
        HOST_CMD_SET_T_ADC,
        HOST_CMD_SET_T_OUT,
        HOST_CMD_BLINDSEARCH,
        HOST_CMD_SAV_MCSU_ENV,
        HOST_CMD_SYS_RESET
    }
    
}
