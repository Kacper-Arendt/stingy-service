namespace Shared.Infrastructure.Communication;

public static class EmailTemplate
{
    public static string Wrap(string body, string appName)
    {
        return $"""
                            <html>
                                <body style='background-color:#f4f4f7;padding:40px 0;'>
                                    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width:600px;background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,0.05);'>
                                        <tr>
                                            <td style='padding:32px 40px 24px 40px;text-align:center;'>
                                                <h2 style='color:#333;font-family:sans-serif;margin-bottom:16px;'>{appName}</h2>
                                                {body}
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='padding:16px 40px 32px 40px;text-align:center;color:#888;font-size:12px;font-family:sans-serif;'>
                                                If you did not expect this invitation, you can safely ignore this email.
                                            </td>
                                        </tr>
                                    </table>
                                </body>
                            </html>
                """;
    }
}