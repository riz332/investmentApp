"use client";

import React, { useCallback, useEffect, useState } from "react";
import CreateAdminForm from "./CreateAdminForm";
import CustomerTable, { Customer } from "./CustomerTable";
import RegisterForm from "./RegisterForm";

interface AdminDashboardProps {
  token: string;
  apiBase: string;
}

export default function AdminDashboard({ token, apiBase }: AdminDashboardProps) {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [authView, setAuthView] = useState<"none" | "register" | "create_admin">("none");

  const loadCustomers = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const headers: HeadersInit = { Authorization: `Bearer ${token}` };
      const res = await fetch(`${apiBase}/api/customer`, { headers });
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      const data = await res.json();
      setCustomers(Array.isArray(data) ? (data as Customer[]) : []);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : String(err);
      setError(msg || "Failed to fetch customers");
    } finally {
      setLoading(false);
    }
  }, [token, apiBase]);

  useEffect(() => {
    loadCustomers();
  }, [loadCustomers]);

  const handleCreationSuccess = (_token: string) => {
    setAuthView("none");
    loadCustomers(); // Refresh customer list
  };

  return (
    <>
      <div className="flex justify-end gap-2 mb-8">
        <button
          onClick={() => setAuthView(authView === "register" ? "none" : "register")}
          className="rounded bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700"
        >
          Create Customer
        </button>
        <button
          onClick={() => setAuthView(authView === "create_admin" ? "none" : "create_admin")}
          className="rounded bg-purple-600 px-4 py-2 text-sm font-medium text-white hover:bg-purple-700"
        >
          Create Admin
        </button>
      </div>

      {authView === "register" && (
        <RegisterForm
          apiBase={apiBase}
          onRegisterSuccess={handleCreationSuccess}
        />
      )}

      {authView === "create_admin" && (
        <CreateAdminForm
          apiBase={apiBase}
          onCreateSuccess={handleCreationSuccess}
        />
      )}

      <CustomerTable customers={customers} loading={loading} error={error} />
    </>
  );
}