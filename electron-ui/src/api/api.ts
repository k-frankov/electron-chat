import axios from "axios";
import { ChatUser } from "../state/models/chatUser";
import { store } from "../state";

export interface AuthResponse {
  user: ChatUser | null;
  errors: string[];
}

export interface ChannelResponse {
  errors: string[];
}

export async function addChannel(channel: string): Promise<ChannelResponse> {
  let errorMessages: string[] = [];
  const token = store.getState()?.authenticatedUser?.jwtToken;
  if (token === undefined) {
    throw Error("User must be authorized.");
  }

  const conf = {
    headers: {
      Authorization: "Bearer " + token,
    },
  };

  await axios
    .post("http://localhost:5100/api/channel", { channelName: channel }, conf)
    .catch((err) => (errorMessages = getErrorMessages(err)));

  return { errors: errorMessages };
}

export async function shareFile(data: Uint8Array): Promise<string[]> {
  let errorMessages: string[] = [];

  if (store.getState().authenticatedUser === null) {
    throw Error("User is not authorized.");
  }
  const dataAsBlob = new Blob([data]);
  const formData = new FormData();
  formData.append('file', dataAsBlob);

  await axios
    .post("http://localhost:5100/api/share", formData, {
      headers: {
        'Content-Type': 'application/octet-stream',
        Authorization: "Bearer " + store.getState().authenticatedUser?.jwtToken,
      }
    })
    .then((resp) => {
      console.log(resp);
    })
    .catch((err) => {
      errorMessages = getErrorMessages(err);
    });
  return errorMessages;
}

export async function signUpApiCall(
  userName: string,
  password: string,
): Promise<AuthResponse> {
  let errorMessages: string[] = [];
  let user: ChatUser | null = null;
  await axios
    .post<ChatUser>("http://localhost:5100/api/auth/register", {
      userName: userName,
      password: password,
    })
    .then((resp) => {
      user = { userName: resp.data.userName, jwtToken: resp.data.jwtToken };
    })
    .catch((err) => {
      errorMessages = getErrorMessages(err);
    });
  return { user: user, errors: errorMessages };
}

export async function signInApiCall(
  userName: string,
  password: string,
): Promise<AuthResponse> {
  let errorMessages: string[] = [];
  let user: ChatUser | null = null;
  await axios
    .post<ChatUser>("http://localhost:5100/api/auth/signin", {
      userName: userName,
      password: password,
    })
    .then((resp) => {
      user = { userName: resp.data.userName, jwtToken: resp.data.jwtToken };
    })
    .catch((err) => {
      errorMessages = getErrorMessages(err);
    });

  return { user: user, errors: errorMessages };
}

function getErrorMessages(err: any): string[] {
  const errorMessages: string[] = [];
  if (err.response) {
    if (typeof err.response.data === "string") {
      errorMessages.push(err.response.data);
    }

    for (const key in err.response.data.errors) {
      const errorValue = err.response.data.errors[key];
      if (typeof errorValue === "string") {
        errorMessages.push(errorValue);
      } else {
        for (const errVal of errorValue) {
          errorMessages.push(errVal);
        }
      }
    }
  } else {
    errorMessages.push(err.message);
  }

  return errorMessages;
}
