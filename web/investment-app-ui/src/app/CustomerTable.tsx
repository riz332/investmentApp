import React from "react";

export interface Customer {
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

interface CustomerTableProps {
  customers: Customer[];
  loading: boolean;
  error: string | null;
}

export default function CustomerTable({ customers, loading, error }: CustomerTableProps) {
  if (loading) {
    return <div>Loading customers…</div>;
  }

  if (error) {
    return <div className="text-red-600">Error: {error}</div>;
  }

  return (
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
  );
}