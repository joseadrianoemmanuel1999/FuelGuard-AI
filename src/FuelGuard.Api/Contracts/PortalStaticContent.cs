namespace FuelGuard.Api.Contracts;

public static class PortalStaticContent
{
    public static IReadOnlyList<LiveFeedEventDto> LiveFeed { get; } =
    [
        new LiveFeedEventDto(
            "[14:22:01]",
            "ALERT",
            "Potential Fuel Siphon Detected",
            "Flow rate mismatch at Terminal Cluster 09 exceeds calibrated variance envelope.",
            ["Node-B12", "AI Conf. 94%"],
            "error"),
        new LiveFeedEventDto(
            "[14:21:48]",
            "ANOMALY",
            "Unusual Credential Access",
            "Failed login attempts from Singapore IP range correlated with admin surface.",
            ["ID: USER_ADMIN_X"],
            "primary"),
        new LiveFeedEventDto(
            "[14:21:12]",
            "RISK",
            "Pressure Sensor Calibration Drift",
            "Maintenance window flagged by AI — reconcile sensor chain before escalation.",
            [],
            "warning"),
        new LiveFeedEventDto(
            "[14:20:55]",
            "INGESTION",
            "Log Batch Sync Complete",
            "Edge gateways 01–14 acknowledged receipt; checksum verified.",
            [],
            "muted"),
        new LiveFeedEventDto(
            "[14:20:01]",
            "ALERT",
            "External API Breach Attempt",
            "Brute force pattern detected from North America subnet — rate limits engaged.",
            [],
            "error"),
    ];

    public static InvestigationsBoardDto InvestigationsBoard { get; } = new(
        Rows:
        [
            new InvestigationBoardRowDto(
                "PetroFlow Corp.",
                "INV-4029-A",
                "CRITICAL HIGH",
                "critical",
                94,
                "Mediterranean SE",
                "Active",
                true,
                "04m ago"),
            new InvestigationBoardRowDto(
                "Helix Logistics",
                "INV-2201-B",
                "MEDIUM",
                "medium",
                62,
                "Red Sea Corridor",
                "Pending",
                false,
                "2h ago"),
            new InvestigationBoardRowDto(
                "Global Bunker Ltd.",
                "INV-1188-C",
                "LOW",
                "low",
                21,
                "Baltic Port North",
                "Pending",
                false,
                "1d ago"),
            new InvestigationBoardRowDto(
                "Vector Energy",
                "INV-7730-D",
                "HIGH",
                "high",
                88,
                "Singapore Hub",
                "Active",
                true,
                "18m ago"),
        ],
        SelectedDetail: new InvestigationDetailDto(
            "INV-4029-A",
            "Neural analysis suggests a high-probability fuel siphoning event. Terminal flow sensors at <span class=\"text-primary font-data-tabular\">Node 42</span> show a 12.4% discrepancy against bill of lading logs. Multiple AIS spoofing events detected within 5nm of the port over the last 72 hours.",
            [
                new InvestigationTimelineItemDto("09:42 UTC", "Sudden Pressure Drop - Node 42", "error"),
                new InvestigationTimelineItemDto("08:15 UTC", "Unrecognized Vessel Signal", "primary"),
                new InvestigationTimelineItemDto("07:00 UTC", "Log Sync Inconsistency Detected", "neutral"),
            ],
            420m,
            368m));

    public static ExecutiveRiskReportDto ExecutiveReport { get; } = new(
        SummaryCards:
        [
            new ExecutiveSummaryCardDto(
                "TOTAL ANOMALIES",
                "1,248",
                "12.4% DECREASE VS PREV. MONTH",
                "primary",
                "analytics"),
            new ExecutiveSummaryCardDto(
                "CRITICAL ALERTS",
                "24",
                "5 REQUIRING IMMEDIATE ACTION",
                "error",
                "emergency_home"),
            new ExecutiveSummaryCardDto(
                "AI CONFIDENCE AVG",
                "92%",
                "",
                "tertiary",
                "model_training"),
            new ExecutiveSummaryCardDto(
                "HIGH-RISK SECTORS",
                "",
                "NORTH-SEA · BALTIC",
                "primary-container",
                "hub"),
        ],
        Sectors:
        [
            new PipelineSectorDto(
                "NS-PL-204",
                "VULNERABLE",
                "error",
                "North Sea Subsea Pipeline (Sector 4)",
                8.4m,
                "ALERT",
                "error"),
            new PipelineSectorDto(
                "BT-TER-09",
                "MONITORING",
                "muted",
                "Baltic Terminal Hub (East Shore)",
                3.2m,
                "SECURE",
                "tertiary"),
            new PipelineSectorDto(
                "AD-REF-81",
                "INVESTIGATING",
                "primary",
                "Adriatic Refining Complex #81",
                5.9m,
                "TRACE",
                "primary"),
            new PipelineSectorDto(
                "MD-ST-002",
                "STABLE",
                "muted",
                "Mediterranean Storage Unit (South)",
                1.1m,
                "SECURE",
                "tertiary"),
        ],
        MapImageUrl:
            "https://lh3.googleusercontent.com/aida-public/AB6AXuAjiaLZohknZDYhRD9E6pvnJzHigCApvlm71LBt3Te_ccY6oBBhFZJy2-tG6xhjzkl0d-vJeLJfarZWq4QunSYiOHI59Ahu6z-t2i4_c9xE22t8P7aX5BXG8Abajk-T4HCCVasFE5FGz3GlM41Q5UY9eBJ9DC8XfzrNpB4hsmKTtwX3XZ_7myHW9JDL9qcHxKDCJ9Lw6uX3O21rswXrbyRZdSOEFUXWmE-QKsGe4xCuJDVR9to3nYlswpYOLbYppoe73a5EiCSn1_z7",
        ChartAnnotation:
            "Major pressure deviation detected at North-Sea Valve #401. 98% AI Confidence.");

    public static TeamDirectoryDto TeamDirectory { get; } = new(
        Members:
        [
            new TeamMemberDto(
                "Marcus Chen",
                "Senior Security Analyst",
                5,
                "ONLINE [HQ-01]",
                "Terminal C4",
                "https://lh3.googleusercontent.com/aida-public/AB6AXuBrod4eEHuV2RAZB3Omf_UPu8a2nXUg_5eH6k9Sup-7wETDc_zeRbaQZrkIMm6jTDg46YL8yi1sElRgPVEssGu1QqCSYFmjpYWmVsn-S8DVtSM1vNCfCj6izA7Wx6SZfOT3wtinW7rPereuO4wN3TwiCJwG9dgHVUXMd9GJPQbC4iE-B-CDL40atDIfCFcBi3I0PsVwFO9sAbpvZV5qqnq0eiZeNYEx2p8uL2LA_rPM3-nfCsKmcN1bIILz5Kfli123tOy84QUdK-9P",
                true,
                ["#INV-9922: Pipeline Latency", "#INV-8812: Ghost Siphoning"]),
            new TeamMemberDto(
                "Sarah Vane",
                "AI Integrity Lead",
                4,
                "SYNCING [REMOTE]",
                "Qwen v4 Core",
                "https://lh3.googleusercontent.com/aida-public/AB6AXuBtv2hc-I2dH-YK7md4SVGdGYP2PkDrcfywxzV7CWH-lfj2Mz9FDCeXV68F3uiTHtr30fxYYd4czDOPzPrbQV3nAlrvA_5UBS5LHK3kByNcu3UxXc09QBkac7OnbDLmT7sDHUDztEOJxIoaNvkiD2t0RwoCS8FZSdfYmquP91yphrH9sfMPG-9MLPPwT1A3ztGCMfVqp8Z5v0k99mq7zMDYvr9oHP4jWM0LedCyHD8fp-Im6RwM3qgS9sjCWQMhFEbKbITJom2riu46",
                true,
                ["#INV-7712: Model Drift Check"]),
            new TeamMemberDto(
                "David Miller",
                "Compliance Officer",
                2,
                "OFFLINE",
                "N/A",
                "https://lh3.googleusercontent.com/aida-public/AB6AXuDD9paG7j1oRvewjqKnJqxpzfZ_vBuEx_IaYGALOxsrBBnHCWo95OPfHTVXroNxPCkGNc3BV0WRYoT5rgUNmeTomQSnIrBLGXn0pC4GSX5TV61WehHzvVpZGb4HpqcRGDIgXxUwA7X-b-Ir4n2Q0w1O-vbMHuPyM0G0Rx3cqXJnY2FUkHH9cutM4s4NZKAYoODmgX2R63bpNX28KJRy1cWaqpRJGI3B-Cnw7dIwQleqr-UqRmTuG2d1cvT7CAbwY_32wnYC9i2orrXV",
                false,
                []),
            new TeamMemberDto(
                "Elena Ross",
                "Field Investigator",
                3,
                "FIELD ACTIVE",
                "Site Delta-9",
                "https://lh3.googleusercontent.com/aida-public/AB6AXuD74SmvnTLzG9-AEJVwuHGIqmAOnTPmmkO33bcGnbJGFoSgiPSbggrwXUAQ4Nf-IsDmSjqm1tQ4mMcnalzXxwgt5-W7VAzRbiySKugcnTJ97TTA7cnOFe-CaYTtV7rd-r0vDpwnP2ZXrSPQ3F-27mM_oLd15GTK8ezXlARopt4x-e83assOhLWsxX9zDO4U__b_rkDTt6_Und5ZfodtmSLTkmq1pLmxH4TTs6uII1QIpb1IM4aYCBjdZ0cX7Q7Sjs3Guu4rHjfumP0j",
                true,
                ["#INV-6610: Physical Tamper"]),
        ],
        AiOversight: new AiOversightPanelDto(
            "Qwen AI Core",
            "ACTIVE",
            98.4m,
            "24",
            "v4.2-PRO"),
        Ledger:
        [
            new AccessLedgerEntryDto("14:22:01", "M. Chen granted Auth #993", "tertiary"),
            new AccessLedgerEntryDto("14:19:44", "S. Vane recalibrated Core", "primary"),
            new AccessLedgerEntryDto("14:15:20", "D. Miller logged out", "muted"),
        ]);

    public static IReadOnlyList<TerminalLogLineDto> TerminalLines { get; } =
    [
        new TerminalLogLineDto("[2023-10-24 14:02:11]", "INFO", "SYS_BOOT", "Vigilance Protocol initialized. Subsystems 01-14 reporting NOMINAL status.", "info"),
        new TerminalLogLineDto("[2023-10-24 14:02:15]", "INFO", "NET_EST", "Secure handshake established with regional data relay node (ID: XR-449).", "info"),
        new TerminalLogLineDto("[2023-10-24 14:03:42]", "WARN", "LAT_SPIKE", "Latency spike detected in Event Bus processing. Current: 420ms. Threshold: 300ms.", "warn"),
        new TerminalLogLineDto("[2023-10-24 14:04:01]", "INFO", "AI_OPT", "AI Core adjusting load balancing parameters. Redistributing compute to Node 04.", "info"),
        new TerminalLogLineDto("[2023-10-24 14:05:18]", "CRIT", "SEC_ANOM", "Unauthorized access attempt detected at Perimeter 07. Protocol 'AEGIS-SHIELD' engaged.", "crit"),
        new TerminalLogLineDto("[2023-10-24 14:05:19]", "INFO", "AUTH_BLK", "IP 192.168.1.104 blacklisted for 3600 seconds. Incident logged to SEC_DB.", "info"),
        new TerminalLogLineDto("[2023-10-24 14:06:05]", "INFO", "TEL_HTBT", "Telemetry heartbeat received from all active edge sensors.", "info-dim"),
        new TerminalLogLineDto("[2023-10-24 14:07:22]", "INFO", "MET_SYNC", "Metric synchronization complete. All time-series data pushed to long-term storage.", "info-dim"),
        new TerminalLogLineDto("[2023-10-24 14:08:44]", "WARN", "RES_WARN", "Disk utilization on Archive-02 exceeding 85%. Automated cleanup scheduled for 02:00.", "warn"),
        new TerminalLogLineDto("[2023-10-24 14:09:12]", "INFO", "JOB_SUCC", "Weekly risk assessment generation completed successfully.", "info"),
        new TerminalLogLineDto("[2023-10-24 14:10:00]", "INFO", "SYS_IDLE", "System entering low-power standby for non-critical monitoring modules.", "info"),
    ];
}
