var plugin = {
  Loaded: function () {},
  SaveExtern: function (data) {
    console.group("SaveExtern");
    var dataString = UTF8ToString(data);
    var jsonUserData = JSON.parse(dataString);

    player.setData(jsonUserData);

    console.log(jsonUserData);
    console.groupEnd();
  },
  LoadExtern: function () {
    console.group("LoadExtern");
    player.getData().then((jsonUserData) => {
      const stringUserData = JSON.stringify(jsonUserData);
      myGameInstance.SendMessage(
        "DataManager",
        "SetPlayerData",
        stringUserData
      );
      console.log(jsonUserData);
      console.log(stringUserData);
    });
    // myGameInstance.SendMessage(
    //   "DataManager",
    //   "SetPlayerData",
    //   JSON.stringify({})
    // );
    console.groupEnd();
  },
  GetUserInfo: function () {
    console.group("GetUserInfo");
    // const myJSON = { name: "TestName", photo: "TestPhoto" };
    const jsonUserInfo = {
      name: player.getName(),
      photo: player.getPhoto("medium"),
    };

    const stringUserInfo = JSON.stringify(jsonUserInfo);

    console.log(jsonUserInfo);
    console.log(stringUserInfo);

    myGameInstance.SendMessage("DataManager", "SetUserInfo", stringUserInfo);
    // myGameInstance.SendMessage(
    //   "DataManager",
    //   "SetUserInfo",
    //   JSON.stringify(myJSON)
    // );
    console.groupEnd();
  },
};

mergeInto(LibraryManager.library, plugin);
