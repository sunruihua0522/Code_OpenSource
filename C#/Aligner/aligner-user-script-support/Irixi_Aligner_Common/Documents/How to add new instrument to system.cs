/*
 * HOW TO ADD NEW INSTRUMENTS TO THE SYSTEM
 * 
 * 1. Add new class which is inherited from #MeasurementInstrumentBase class.
 *   > Simpling using the #new_instrument.cs template file to create the new instrument class.
 *   > The .cs file should be put in the following path: /Equipments
 * 
 * 2. Add new view class for the new instrument
 *   > The .cs file should be put in the following path: /ViewModel
 * 
 * 3. Add new configuration class which is inherited from #ConfigurationBase class.
 *   > The .cs file should be put in the following folder: /Configuration/Equipments
 *   
 * 4. Add new configuration field to the #SystemSetting.json file
 * 
 * 5. Create the instance of the new instrument to the #MeasurementInstrumentCollection in the #SystemService class
 * 
 * 5. Create a UserControl to operate the instrument
 * 
 * 6. Create the panel to control the instrument in the constructor of the MainWindow
 * 
 */