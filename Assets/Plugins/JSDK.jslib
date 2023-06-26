var plugin = {
  Loaded: function () {},
  SaveExtern: function (data) {
    var dataString = UTF8ToString(data);
    var myObj = JSON.parse();

    // player.setData(myObj);
    console.group("Save");
    console.log(myObj);
    console.groupEnd();
  },
  LoadExtern: function () {
    // player.getData().then((_data) => {
    //   const myJSON = JSON.stringify(_data);
    //   myGameInstance.SendMessage("DataManager", "LoadPlayerData", myJSON);
    // });
    console.group("Load");
    // console.log(myObj);
    console.groupEnd();
  },
};

mergeInto(LibraryManager.library, plugin);
