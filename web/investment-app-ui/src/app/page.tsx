"use client";

import Image from "next/image";
import React, { useEffect, useState } from "react";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";
import CreateAdminForm from "./CreateAdminForm";
import CustomerTable, { Customer } from "./CustomerTable";

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5002";

export default function Home() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [authView, setAuthView] = useState<"none" | "login" | "register" | "create_admin">("none");

  useEffect(() => {
    let mounted = true;

    async function loadCustomers() {
      setLoading(true);
      setError(null);
      try {
        const headers: HeadersInit = {};
        if (token) {
          headers["Authorization"] = `Bearer ${token}`;
        }

        const res = await fetch(`${API_BASE}/api/customer`, { headers });
        if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
        const data = await res.json();
        if (mounted) {
          setCustomers(Array.isArray(data) ? (data as Customer[]) : []);
        }
      } catch (err: unknown) {
        const msg = err instanceof Error ? err.message : String(err);
        if (mounted) setError(msg || "Failed to fetch customers");
      } finally {
        if (mounted) setLoading(false);
      }
    }

    loadCustomers();
    return () => {
      mounted = false;
    };
  }, [token]);

  const handleAuthSuccess = (newToken: string) => {
    setToken(newToken);
    setAuthView("none");
  };

  return (
    <div className="flex min-h-screen items-start justify-center bg-zinc-50 font-sans dark:bg-black py-12">
      <main className="w-full max-w-5xl px-6">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-4">
            <Image src="/next.svg" alt="Next.js" width={80} height={20} />
            <h1 className="text-2xl font-semibold">Customers</h1>
          </div>
          <div className="flex gap-2">
            {!token ? (
              <>
                <button
                  onClick={() => setAuthView(authView === "login" ? "none" : "login")}
                  className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
                >
                  Login
                </button>
                <button
                  onClick={() => setAuthView(authView === "register" ? "none" : "register")}
                  className="rounded bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700"
                >
                  Register
                </button>
                <button
                  onClick={() => setAuthView(authView === "create_admin" ? "none" : "create_admin")}
                  className="rounded bg-purple-600 px-4 py-2 text-sm font-medium text-white hover:bg-purple-700"
                >
                  Create Admin
                </button>
              </>
            ) : (
              <button
                onClick={() => setToken(null)}
                className="rounded bg-gray-600 px-4 py-2 text-sm font-medium text-white hover:bg-gray-700"
              >
                Logout
              </button>
            )}
          </div>
        </div>

        {authView === "login" && !token && (
          <LoginForm apiBase={API_BASE} onLoginSuccess={handleAuthSuccess} />
        )}

        {authView === "register" && !token && (
          <RegisterForm apiBase={API_BASE} onRegisterSuccess={handleAuthSuccess} />
        )}

        {authView === "create_admin" && !token && (
          <CreateAdminForm apiBase={API_BASE} onCreateSuccess={handleAuthSuccess} />
        )}

        <CustomerTable customers={customers} loading={loading} error={error} />
      </main>
    </div>
  );
}
