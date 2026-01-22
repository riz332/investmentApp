import Image from "next/image";
import React, { useEffect, useState } from "react";

interface Customer {
  customerId?: string;
  CustomerId?: string;
  firstName?: string;
  FirstName?: string;
  lastName?: string;
  LastName?: string;
  email?: string;
  Email?: string;
  phone?: string;
  Phone?: string;
  createdAt?: string;
  CreatedAt?: string;
}

export default function Home() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [authView, setAuthView] = useState<"none" | "login" | "register">("none");

  // Login Form State
  const [loginEmail, setLoginEmail] = useState("");
  const [loginPassword, setLoginPassword] = useState("");

  // Register Form State
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");
  const [regFirstName, setRegFirstName] = useState("");
  const [regLastName, setRegLastName] = useState("");

  const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5002";

  useEffect(() => {
    let mounted = true;

    async function loadCustomers() {
      try {
        const res = await fetch(`${API_BASE}/api/customer`);
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
  }, []);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await fetch(`/api/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: loginEmail, password: loginPassword }),
      });
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || res.statusText);
      }
      const data = await res.json();
      setToken(data.token);
      setAuthView("none");
      setLoginEmail("");
      setLoginPassword("");
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : String(err));
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await fetch(`/api/Auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: regEmail,
          password: regPassword,
          firstName: regFirstName,
          lastName: regLastName,
        }),
      });
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || res.statusText);
      }
      const data = await res.json();
      setToken(data.token);
      setAuthView("none");
      setRegEmail("");
      setRegPassword("");
      setRegFirstName("");
      setRegLastName("");
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : String(err));
    }
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
          <div className="mb-8 rounded-md border border-gray-200 bg-white p-6 shadow-sm">
            <h2 className="mb-4 text-lg font-semibold">Login</h2>
            <form onSubmit={handleLogin} className="space-y-4 max-w-md">
              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <input
                  type="email"
                  required
                  value={loginEmail}
                  onChange={(e) => setLoginEmail(e.target.value)}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Password</label>
                <input
                  type="password"
                  required
                  value={loginPassword}
                  onChange={(e) => setLoginPassword(e.target.value)}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
              <button
                type="submit"
                className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
              >
                Sign In
              </button>
            </form>
          </div>
        )}

        {authView === "register" && !token && (
          <div className="mb-8 rounded-md border border-gray-200 bg-white p-6 shadow-sm">
            <h2 className="mb-4 text-lg font-semibold">Register</h2>
            <form onSubmit={handleRegister} className="space-y-4 max-w-md">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">First Name</label>
                  <input
                    type="text"
                    value={regFirstName}
                    onChange={(e) => setRegFirstName(e.target.value)}
                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-green-500 focus:outline-none focus:ring-1 focus:ring-green-500"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Last Name</label>
                  <input
                    type="text"
                    value={regLastName}
                    onChange={(e) => setRegLastName(e.target.value)}
                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-green-500 focus:outline-none focus:ring-1 focus:ring-green-500"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <input
                  type="email"
                  required
                  value={regEmail}
                  onChange={(e) => setRegEmail(e.target.value)}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-green-500 focus:outline-none focus:ring-1 focus:ring-green-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Password</label>
                <input
                  type="password"
                  required
                  value={regPassword}
                  onChange={(e) => setRegPassword(e.target.value)}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-green-500 focus:outline-none focus:ring-1 focus:ring-green-500"
                />
              </div>
              <button
                type="submit"
                className="rounded bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700"
              >
                Sign Up
              </button>
            </form>
          </div>
        )}

        {loading ? (
          <div>Loading customers…</div>
        ) : error ? (
          <div className="text-red-600">Error: {error}</div>
        ) : (
          <div className="overflow-x-auto rounded-md border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Name</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Email</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Phone</th>
                  <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Created</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100 bg-white">
                {customers.map((c: Customer, i: number) => {
                  const id = (c.customerId ?? c.CustomerId) ?? String(i);
                  const first = c.firstName ?? c.FirstName ?? "";
                  const last = c.lastName ?? c.LastName ?? "";
                  const email = c.email ?? c.Email ?? "";
                  const phone = c.phone ?? c.Phone ?? "";
                  const created = c.createdAt ?? c.CreatedAt ?? null;

                  return (
                    <tr key={id} className="hover:bg-gray-50">
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-900">{first} {last}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">{email}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">{phone}</td>
                      <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-500">{created ? new Date(created).toLocaleString() : "-"}</td>
                    </tr>
                  );
                })}
                {customers.length === 0 && (
                  <tr>
                    <td colSpan={4} className="px-6 py-4 text-center text-sm text-gray-500">No customers found.</td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </main>
    </div>
  );
}
