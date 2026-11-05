"use client";

import { useCallback, useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/api/config";
import { probeEndpoint, type ProbeResult } from "@/lib/api/probe";
import { REMOTE_CHECKS, type RemoteCheckId } from "./checks";

type Results = Record<RemoteCheckId, ProbeResult>;

const INITIAL: Results = {
  servidor: { state: "checking" },
  baseDeDatos: { state: "checking" },
};

export function useRemoteChecks() {
  const [results, setResults] = useState<Results>(INITIAL);
  const [round, setRound] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    for (const check of REMOTE_CHECKS) {
      probeEndpoint(`${API_BASE_URL}${check.path}`, controller.signal).then((result) => {
        if (!controller.signal.aborted) {
          setResults((previous) => ({ ...previous, [check.id]: result }));
        }
      });
    }

    return () => controller.abort();
  }, [round]);

  const recheck = useCallback(() => {
    setResults(INITIAL);
    setRound((current) => current + 1);
  }, []);

  return { results, recheck };
}
