import axios from "axios";

export async function signUpApiCall(
  userName: string,
  password: string,
): Promise<string[]> {
  const errorMessages: string[] = [];
  await axios
    .post("http://localhost:5000/api/auth/register", {
      userName: userName,
      password: password,
    })
    .then((resp) => {
      console.log(resp);
    })
    .catch((err) => {
      if (err.response) {
        console.log(err.response);
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
    });
  return errorMessages;
}

export async function signInApiCall(
  userName: string,
  password: string,
): Promise<string[]> {
  let errorMessages: string[] = [];
  await axios
    .post("http://localhost:5000/api/auth/signin", {
      userName: userName,
      password: password,
    })
    .then((resp) => {
      console.log(resp);
    })
    .catch((err) => {
      errorMessages = getErrorMessages(err);
    });
  return errorMessages;
}

function getErrorMessages(err: any): string[] {
  const errorMessages: string[] = [];
  if (err.response) {
    console.log(err.response);
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
