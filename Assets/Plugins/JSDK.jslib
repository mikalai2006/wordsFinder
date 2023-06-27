var plugin = {
  Loaded: function () {},
  SaveExtern: function (data) {
    var dataString = UTF8ToString(data);
    var myObj = JSON.parse(dataString);

    // player.setData(myObj);
    console.group("Save");
    console.log(myObj);
    console.groupEnd();
  },
  LoadExtern: function () {
    console.log("Load data");
    // player.getData().then((_data) => {
    //   const myJSON = JSON.stringify(_data);
    //   myGameInstance.SendMessage("DataManager", "SetPlayerData", myJSON);
    // });
    myGameInstance.SendMessage(
      "DataManager",
      "SetPlayerData",
      JSON.stringify({})
    );
  },
  GetPhoto: function () {
    console.log("GetPhoto");
    // player.getPhoto().then((_data) => {
    //   const myJSON = JSON.stringify(_data);
    //   myGameInstance.SendMessage("DataManager", "SetPhoto", myJSON);
    // });
    myGameInstance.SendMessage("DataManager", "SetPhoto", "photo");
  },
  GetName: function () {
    console.log("GetName");
    // player.getName().then((_data) => {
    //   const myJSON = JSON.stringify(_data);
    //   myGameInstance.SendMessage("DataManager", "SetName", myJSON);
    // });
    myGameInstance.SendMessage("DataManager", "SetName", "Mikki");
  },
};

mergeInto(LibraryManager.library, plugin);
