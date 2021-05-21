import {
  Alert,
  AlertIcon,
  AlertTitle,
  Box,
  Button,
  FormControl,
  Input,
  InputGroup,
  InputLeftElement,
  Spinner,
  Stack,
} from "@chakra-ui/react";
import { FaUserAlt, FaLock } from "react-icons/fa";
import { signUpApiCall } from "../../api/authApi";
import { useDispatch } from "react-redux";
import { bindActionCreators } from "redux";
import { actionCreators } from "../../state";

import React from "react";

const SignUp = (): JSX.Element => {
  const [userName, setUserName] = React.useState<string>("");
  const [password, setPassword] = React.useState<string>("");
  const [errors, setErrors] = React.useState<string[]>([]);
  const [inProgress, setInProgress] = React.useState<boolean>(false);

  const dispatch = useDispatch();
  const { logIn } = bindActionCreators(actionCreators, dispatch);

  async function doSignUp() {
    setInProgress(true);
    const errorsFromApi = await signUpApiCall(userName, password);
    if (errorsFromApi === undefined || errorsFromApi.length === 0) {
      logIn();
    } else {
      setErrors(errorsFromApi);
      setUserName("");
      setPassword("");
      setInProgress(false);
    }
  }

  return (
    <Box minW={{ base: "90%", md: "468px" }}>
      <form>
        <Stack spacing={4} p="1rem" boxShadow="md">
          <FormControl>
            <InputGroup>
              <InputLeftElement pointerEvents="none" children={<FaUserAlt />} />
              <Input
                type="text"
                placeholder="user name"
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
              />
            </InputGroup>
          </FormControl>
          <FormControl>
            <InputGroup>
              <InputLeftElement pointerEvents="none" children={<FaLock />} />
              <Input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </InputGroup>
          </FormControl>
          {inProgress ? (
            <Spinner alignSelf="center" />
          ) : (
            <Button
              borderRadius={0}
              type="submit"
              variant="solid"
              colorScheme="teal"
              width="full"
              onClick={doSignUp}
            >
              Sign Up
            </Button>
          )}
        </Stack>
        <ul style={{ listStyleType: "none" }}>
          {errors.map((err, i) => (
            <li key={i}>
              <Alert status="error">
                <AlertIcon />
                <AlertTitle mr={2}>{err}</AlertTitle>
              </Alert>
            </li>
          ))}
        </ul>
      </form>
    </Box>
  );
};

export default SignUp;
