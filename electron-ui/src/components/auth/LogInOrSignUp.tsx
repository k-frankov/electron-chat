import { Box, Heading, Link, VStack } from "@chakra-ui/react";
import React, { useState } from "react";
import ReactCardFlip from "react-card-flip";
import LogIn from "./LogIn";
import SignUp from "./SignUp";

const LogInOrSignup = (): JSX.Element => {
  const [isSignUp, setIsSignUp] = useState(false);

  return (
    <VStack justifyContent="center" alignItems="center" minH="75vh">
      <Heading color="teal.400">{isSignUp ? "Thank you for registering!" : "Welcome back!"}</Heading>
      <ReactCardFlip isFlipped={isSignUp} flipDirection="horizontal">
        <LogIn />
        <SignUp />
      </ReactCardFlip>
      {isSignUp ? (
        <Box>
          Alread have account?{" "}
          <Link
            color="teal.500"
            href="#"
            onClick={() => {
              setIsSignUp(false);
            }}
          >
            Log In
          </Link>
        </Box>
      ) : (
        <Box>
          New to us?{" "}
          <Link
            color="teal.500"
            href="#"
            onClick={() => {
              setIsSignUp(true);
            }}
          >
            Sign Up
          </Link>
        </Box>
      )}
    </VStack>
  );
};

export default LogInOrSignup;
