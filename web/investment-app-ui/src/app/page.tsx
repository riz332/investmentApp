"use client";

import Image from "next/image";
import React, { useEffect, useState } from "react";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";
import { Customer } from "./CustomerTable";
import { parseJwt } from "./jwt-utils";
import AdminDashboard from "./AdminDashboard";
import CustomerDashboard from "./CustomerDashboard";

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5002";

export default function Home() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<Customer | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const [authView, setAuthView] = useState<"none" | "login" | "register">("none");

  useEffect(() => {
    async function fetchUserData(userId: string) {
    setLoading(true);
    setError(null);
    try {
      const headers: HeadersInit = { Authorization: `Bearer ${token}` };
      const res = await fetch(`${API_BASE}/api/customer/${userId}`, { headers });
      if (!res.ok) throw new Error(`Failed to fetch user data: ${res.status} ${res.statusText}`);
      const data = await res.json();
      setUser(data);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : String(err);
      setError(msg);
    } finally {
      setLoading(false);
    }
  }

    if (role === "User" && token) {
      const decodedToken = parseJwt(token);
      const userId = decodedToken?.nameid;
      if (userId) {
        fetchUserData(userId);
      }
    }
  }, [token, role]);

  const handleAuthSuccess = (newToken: string) => {
    setToken(newToken);
    const decoded = parseJwt(newToken);
    const userRole = decoded?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    setRole(userRole);
    setAuthView("none");
  };

  const handleLogout = () => {
    setToken(null);
    setUser(null);
    setRole(null);
  };

  return (
    <div className="flex min-h-screen items-start justify-center bg-zinc-50 font-sans dark:bg-black py-12">
      <main className="w-full max-w-5xl px-6">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-4">
            <Image src="/next.svg" alt="Next.js" width={80} height={20} />
            <h1 className="text-2xl font-semibold">InvestmentApp</h1>
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
              </>
            ) : (
              <button
                onClick={handleLogout}
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

        {token && role === "Admin" && (
          <AdminDashboard token={token} apiBase={API_BASE} />
        )}

        {token && role === "User" && (
          <>
            {loading && <div>Loading your details...</div>}
            {error && <div className="text-red-600">Error: {error}</div>}
            {user && <CustomerDashboard user={user} />}
          </>
        )}
      </main>
    </div>
  );
}
