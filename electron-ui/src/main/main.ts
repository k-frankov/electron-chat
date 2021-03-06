/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { app, BrowserWindow } from "electron";
import installExtension, {
  REACT_DEVELOPER_TOOLS,
} from "electron-devtools-installer";
const { ipcMain } = require('electron');
const fs = require('fs')

declare global {
  const MAIN_WINDOW_WEBPACK_ENTRY: string;
}

// Handle creating/removing shortcuts on Windows when installing/uninstalling.
if (require("electron-squirrel-startup")) {
  // eslint-disable-line global-require
  app.quit();
}

app.whenReady().then(() => {
  installExtension(REACT_DEVELOPER_TOOLS, {
    loadExtensionOptions: { allowFileAccess: true },
    forceDownload: false,
  })
    .then((name) => console.log(`Added Extension:  ${name}`))
    .catch((err) => console.log("An error occurred: ", err));
});

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow: null | BrowserWindow;

const createWindow = () => {
  // Create the browser window.
  mainWindow = new BrowserWindow({
    minWidth: 600,
    minHeight: 400,
    width: 800,
    height: 600,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
  });

  // and load the index.html of the app.
  mainWindow.loadURL(MAIN_WINDOW_WEBPACK_ENTRY);

  // Open the DevTools.
  mainWindow!.webContents.openDevTools();


  // Emitted when the window is closed.
  mainWindow.on("closed", () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on("ready", createWindow);

// Quit when all windows are closed.
app.on("window-all-closed", () => {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (mainWindow === null) {
    createWindow();
  }
});

ipcMain.on('openExternalLink', (event, link) => {
  console.log("hello", link);
  require("electron").shell.openExternal(link);
})

ipcMain.on('openFile', (event, path) => {
  const { dialog } = require('electron')
  if (mainWindow !== null) {
    dialog.showOpenDialog(mainWindow)
      .then((res) => {
        if (res.filePaths.length > 0) {
          readFile(res.filePaths[0]);
        }
      })
      .catch(err => console.log(err))
  }
});

function readFile(filepath: string) {
  fs.readFile(filepath, (err: any, data: any) => {
    if (err) {
      console.log(err)
      return;
    }

   const pathArray = process.platform === "win32" ? filepath.split("\\") : filepath.split("/");
   const fileName =  pathArray[pathArray.length - 1];
   
    mainWindow?.webContents.send("fileData", data, fileName);
  })
}

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and import them here.
